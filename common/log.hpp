#pragma once
#include "common/macro_define.hpp"
#include "common/concurrent_queue.hpp"
#include "common/time.hpp"
#include "common/termcolor.hpp"
#include "common/directory.hpp"
#include "common/object_pool.hpp"
#include "common/buffer.hpp"
#include "common/spinlock.hpp"

namespace moon
{
    enum class LogLevel
    {
        Error = 1,
        Warn,
        Info,
        Debug,
        Max
    };

    class log
    {
        static const int MAX_LOG_LEN = 8 * 1024;
    public:
        log()
            :state_(state::init)
            , level_(LogLevel::Debug)
            , thread_(&log::write, this)
        {
        }

        ~log()
        {
            wait();
        }

        log(const log&) = delete;

        log& operator=(const log&) = delete;

        void init(const std::string& logfile)
        {
            if (ofs_)
            {
                ofs_->flush();
                ofs_->close();
            }

            if (!logfile.empty())
            {
                std::error_code ec;
                auto parent_path = fs::path(logfile).parent_path();
                if (!fs::exists(parent_path, ec))
                {
                    fs::create_directories(parent_path, ec);
                    MOON_CHECK(!ec, ec.message().data());
                }
                ofs_.reset(new std::ofstream());
                ofs_->open(logfile, std::ofstream::out | std::ofstream::trunc);
            }
            state_.store(state::ready, std::memory_order_release);
        }

        void logfmt(bool console, LogLevel level, const char* fmt, ...)
        {
            if (level_ < level)
            {
                return;
            }

            if (nullptr == fmt)
                return;

            char fmtbuf[MAX_LOG_LEN];
            va_list ap;
            va_start(ap, fmt);
#if TARGET_PLATFORM == PLATFORM_WINDOWS
            int n = vsnprintf_s(fmtbuf, MAX_LOG_LEN, fmt, ap);
#else
            int n = vsnprintf(fmtbuf, MAX_LOG_LEN, fmt, ap);
#endif
            va_end(ap);
            logstring(console, level, moon::string_view_t(fmtbuf, n));
        }

        void logstring(bool console, LogLevel level, moon::string_view_t s)
        {
            if (level_ < level)
            {
                return;
            }

            auto buf = buffer_cache_.create(s.size());
            auto b = std::addressof(*buf->begin());
            *(b++) = static_cast<char>(console);
            *(b++) = static_cast<char>(level);
            size_t offset = format_header(b, level);
            buf->offset_writepos(2 + static_cast<int>(offset));
            buf->write_back(s.data(), 0, s.size());
            buf->write_back("\n", 0, 1);
            log_queue_.push_back(buf);
        }

        void set_level(LogLevel level)
        {
            level_ = level;
        }

        void set_level(string_view_t s)
        {
            if (moon::iequal_string(s, string_view_t{ "DEBUG" }))
            {
                set_level(LogLevel::Debug);
            }
            else  if (moon::iequal_string(s, string_view_t{ "INFO" }))
            {
                set_level(LogLevel::Info);
            }
            else  if (moon::iequal_string(s, string_view_t{ "WARN" }))
            {
                set_level(LogLevel::Warn);
            }
            else  if (moon::iequal_string(s, string_view_t{ "ERROR" }))
            {
                set_level(LogLevel::Error);
            }
            else
            {
                set_level(LogLevel::Debug);
            }
        }

        void wait()
        {
            if (state_.load() == state::exited)
            {
                return;
            }

            state_.store(state::exited);
            log_queue_.exit();

            if (thread_.joinable())
                thread_.join();

            if (ofs_&&ofs_->is_open())
            {
                ofs_->flush();
                ofs_->close();
                ofs_ = nullptr;
            }
        }
    private:
        size_t format_header(char* buf, LogLevel level) const
        {
            size_t offset = 0;
            offset += time::milltimestamp(time::now(), buf, 23);
            memcpy(buf + offset, " | ", 3);
            offset += 3;
            auto len = moon::uint64_to_str(moon::thread_id(), buf + offset);
            offset += len;
            if (len < 6)
            {
                memcpy(buf + offset, "      ", 6 - len);
                offset += 6 - len;
            }
            memcpy(buf + offset, to_string(level), 11);
            offset += 11;
            return offset;
        }

        void write()
        {
            while (state_.load(std::memory_order_acquire) == state::init)
                std::this_thread::sleep_for(std::chrono::microseconds(50));

            queue_t::container_type swaps;
            while (state_.load() == state::ready || log_queue_.size() != 0)
            {
                swaps.clear();
                log_queue_.swap(swaps);
                for (auto& it : swaps)
                {
                    auto b = it->data();
                    auto bconsole = static_cast<bool>(*(b++));
                    auto level = static_cast<LogLevel>(*(b++));
                    it->seek(2, buffer::seek_origin::Current);
                    if (bconsole)
                    {
                        auto s = moon::string_view_t(reinterpret_cast<const char*>(it->data()), it->size());
                        switch (level)
                        {
                        case LogLevel::Error:
                            std::cerr << termcolor::red << s;
                            break;
                        case LogLevel::Warn:
                            std::cout << termcolor::yellow << s;
                            break;
                        case LogLevel::Info:
                            std::cout << termcolor::white << s;
                            break;
                        case LogLevel::Debug:
                            std::cout << termcolor::green << s;
                            break;
                        default:
                            break;
                        }
                        std::cout << termcolor::white;
                    }

                    if (ofs_)
                    {
                        ofs_->write(it->data(), it->size());
                    }
                }
                if (ofs_)
                {
                    ofs_->flush();
                }
            }
        }

        const char* to_string(LogLevel lv) const
        {
            switch (lv)
            {
            case LogLevel::Error:
                return " | ERROR | ";
            case LogLevel::Warn:
                return " | WARN  | ";
            case LogLevel::Info:
                return " | INFO  | ";
            case LogLevel::Debug:
                return " | DEBUG | ";
            default:
                return " | NULL  | ";
            }
        }

        std::atomic<state> state_;
        std::atomic<LogLevel> level_;
        std::unique_ptr<std::ofstream > ofs_;
        std::thread thread_;
        shared_pointer_pool<buffer, 1000, spin_lock> buffer_cache_;
        using queue_t = concurrent_queue<std::shared_ptr<buffer>, std::mutex, std::vector, true>;
        queue_t log_queue_;
    };

#define CONSOLE_INFO(logger,fmt,...) logger->logfmt(true,moon::LogLevel::Info,fmt,##__VA_ARGS__);
#define CONSOLE_WARN(logger,fmt,...) logger->logfmt(true,moon::LogLevel::Warn,fmt" (%s:%d)",##__VA_ARGS__,__FILENAME__,__LINE__);
#define CONSOLE_ERROR(logger,fmt,...) logger->logfmt(true,moon::LogLevel::Error,fmt" (%s:%d)",##__VA_ARGS__,__FILENAME__,__LINE__);

#define LOG_INFO(logger,fmt,...) logger->logfmt(false,moon::LogLevel::Info,fmt,##__VA_ARGS__);  
#define LOG_WARN(logger,fmt,...) logger->logfmt(false,moon::LogLevel::Warn,fmt" (%s:%d)",##__VA_ARGS__,__FILENAME__,__LINE__);
#define LOG_ERROR(logger,fmt,...) logger->logfmt(false,moon::LogLevel::Error,fmt" (%s:%d)",##__VA_ARGS__,__FILENAME__,__LINE__);

#define CONSOLE_DEBUG(logger,fmt,...) logger->logfmt(true,moon::LogLevel::Debug,fmt" (%s:%d)",##__VA_ARGS__,__FILENAME__,__LINE__);
#define LOG_DEBUG(logger,fmt,...) logger->logfmt(false,moon::LogLevel::Debug,fmt" (%s:%d)",##__VA_ARGS__,__FILENAME__,__LINE__);
}



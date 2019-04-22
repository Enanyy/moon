#pragma once
#include "base_connection.hpp"
#include "common/byte_convert.hpp"

namespace moon
{
    class moon_connection : public base_connection
    {
    public:
        static constexpr message_size_t MASK_CONTINUED = 0x8000;
        static constexpr message_size_t MASK_SIZE = 0x7FFF;
        static constexpr message_size_t MAX_MSG_FRAME_SIZE = MAX_NET_MSG_SIZE - sizeof(message_size_t);

        using base_connection_t = base_connection;

        template <typename... Args>
        explicit moon_connection(frame_enable_flag flag, Args&&... args)
            :base_connection(std::forward<Args>(args)...)
            , frame_flag_(flag)
            , header_(0)
        {
        }

        void start(bool accepted, int32_t responseid = 0) override
        {
            base_connection_t::start(accepted, responseid);
            auto msg = message::create();
            msg->write_data(remote_addr_);
            msg->set_subtype(static_cast<uint8_t>(accepted ? socket_data_type::socket_accept : socket_data_type::socket_connect));
            handle_message(std::move(msg));
            read_header();
        }

        bool send(const buffer_ptr_t & data) override
        {
            if (!data->has_flag(buffer_flag::pack_size))
            {
                if (data->size() > MAX_MSG_FRAME_SIZE)
                {
                    bool enable = (static_cast<int>(frame_flag_)&static_cast<int>(frame_enable_flag::send)) != 0;
                    if (!enable)
                    {
                        error(asio::error_code(), int(network_logic_error::send_message_size_max));
                        base_connection_t::close();
                        return false;
                    }
                    data->set_flag(buffer_flag::framing);
                }
                else
                {
                    message_size_t size = static_cast<message_size_t>(data->size());
                    host2net(size);
                    [[maybe_unused]]  bool res = data->write_front(&size, 0, 1);
                    MOON_ASSERT(res, "tcp::send write front failed");
                    data->set_flag(buffer_flag::pack_size);
                }
            }
            return base_connection_t::send(data);
        }
    protected:
        void message_framing(const_buffers_holder& holder, buffer_ptr_t&& buf) override
        {
            size_t n = buf->size();
            holder.framing_begin(n / MAX_NET_MSG_SIZE + 1);
            do
            {
                message_size_t  size = 0, header = 0;
                if (n > MAX_NET_MSG_SIZE)
                {
                    header = size = MAX_NET_MSG_SIZE;
                    header |= MASK_CONTINUED;
                }
                else
                {
                    header = size = static_cast<message_size_t>(n);
                }
                const char* data = buf->data() + (buf->size() - n);
                n -= size;
                host2net(header);
                holder.push_framing(header, data, size);
            } while (n != 0);
            holder.framing_end(std::forward<buffer_ptr_t>(buf));
        }

        void read_header()
        {
            asio::async_read(socket_, asio::buffer(&header_, sizeof(header_)),
                make_custom_alloc_handler(read_allocator_,
                    [this, self = shared_from_this()](const asio::error_code& e, std::size_t bytes_transferred)
            {
                if (e)
                {
                    error(e, int(logic_error_));
                    return;
                }

                if (bytes_transferred == 0)
                {
                    read_header();
                    return;
                }

                last_recv_time_ = now();
                net2host(header_);

                bool enable = (static_cast<int>(frame_flag_)&static_cast<int>(frame_enable_flag::receive)) != 0;
                bool continued = false;
                if (enable)
                {
                    //check is continued message
                    continued = ((header_ & MASK_CONTINUED) != 0);
                    if (continued)
                    {
                        header_ &= MASK_SIZE;
                    }
                }

                if (header_ > MAX_NET_MSG_SIZE)
                {
                    error(asio::error_code(), int(network_logic_error::read_message_size_max));
                    base_connection_t::close();
                    return;
                }
                read_body(header_, continued);
            }));
        }

        void read_body(message_size_t size, bool continued)
        {
            if (nullptr == buf_)
            {
                buf_ = message::create_buffer(continued ? 5 * size : size);
            }
            else
            {
                buf_->check_space(size);
            }

            asio::async_read(socket_, asio::buffer((buf_->data() + buf_->size()), size),
                make_custom_alloc_handler(read_allocator_,
                    [this, self = shared_from_this(), continued](const asio::error_code& e, std::size_t bytes_transferred)
            {
                if (e)
                {
                    error(e, int(logic_error_));
                    return;
                }

                if (bytes_transferred == 0)
                {
                    read_header();
                    return;
                }

                buf_->offset_writepos(static_cast<int>(bytes_transferred));
                if (!continued)
                {
                    auto msg = message::create(std::move(buf_));
                    msg->set_subtype(static_cast<uint8_t>(socket_data_type::socket_recv));
                    handle_message(std::move(msg));
                }
                read_header();
            }));
        }

    protected:
        frame_enable_flag frame_flag_;
        message_size_t header_;
        buffer_ptr_t buf_;
    };
}
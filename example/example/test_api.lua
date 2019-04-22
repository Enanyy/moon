local moon = require("moon")
local log = require("moon.log")
local json = require("json")
local seri = require("seri")
local fs = require("fs")
local test_assert = require("test_assert")

local equal = test_assert.equal

equal(type(moon.LOGV) ,"function")
equal(log.LOG_ERROR , 1)
equal(log.LOG_WARN ,2)
equal(log.LOG_INFO , 3)
equal(log.LOG_DEBUG , 4)

equal(type(checkbool) , "function")
equal(type(checkint) , "function")
equal(type(checknumber) , "function")
equal(type(checktable) , "function")

equal(type(class) , "function")
equal(type(iskindof) , "function")

equal(type(moon.name) , "function")
equal(type(moon.id) , "function")
equal(type(moon.send_prepare) , "function")
equal(type(moon.prepare) , "function")
equal(type(moon.broadcast) , "function")
equal(type(moon.get_tcp) , "function")
equal(type(moon.unique_service) , "function")
equal(type(moon.new_service) , "function")
equal(type(moon.send) , "function")
equal(type(moon.set_cb) , "function")
equal(type(moon.workernum) , "function")
equal(type(moon.abort) , "function")

local msg = moon.message
equal(type(msg.sender) , "function")
equal(type(msg.responseid) , "function")
equal(type(msg.receiver) , "function")
equal(type(msg.type) , "function")
equal(type(msg.subtype) , "function")
equal(type(msg.header) , "function")
equal(type(msg.bytes) , "function")
equal(type(msg.size) , "function")
equal(type(msg.substr) , "function")
equal(type(msg.buffer) , "function")
equal(type(msg.redirect) , "function")
equal(type(msg.resend) , "function")

equal(type(fs.create_directory) , "function")
equal(type(fs.working_directory) , "function")
print(fs.working_directory())
equal(type(fs.exists) , "function")
equal(type(fs.traverse_folder) , "function")

local tcp = moon.tcp
equal(type(tcp.async_accept) , "function")
equal(type(tcp.async_connect) , "function")
equal(type(tcp.listen) , "function")
equal(type(tcp.close) , "function")
equal(type(tcp.connect) , "function")
equal(type(tcp.read) , "function")
equal(type(tcp.send) , "function")
equal(type(tcp.send_message) , "function")
equal(type(tcp.settimeout) , "function")
equal(type(moon.millsecond) , "function")

local timer = moon
equal(type(timer.remove_timer) , "function")
equal(type(timer.repeated) , "function")

equal(type(moon.millsecond) , "function")
equal(type(moon.sleep) , "function")
equal(type(string.hash) , "function")
equal(type(string.hex) , "function")
equal(type(table.new) , "function")

local nt = table.new(10,1)
nt[1] = 10

print("*********************api test ok**********************")

moon.prepare(nil)
moon.prepare("123")
moon.prepare(seri.pack("1",2,3,{a=1,b=2},nil))

moon.set_env("1","2")
equal(moon.get_env("1"),"2")
moon.set_env("1","3")
equal(moon.get_env("1"),"3")

local Base = class("Base")
function Base:ctor()
end

local Child = class("Child", Base)
function Child:ctor()
	Child.super.ctor(self)
end

local ChildB = class("ChildB", Base)
function ChildB:ctor()
	ChildB.super.ctor(self)
end

local c = Child.new()
local cb = ChildB.new()

equal(iskindof(c, "Child"),true)
equal(iskindof(c, "Base"),true)
equal(iskindof(c, "ChildB"),false)
equal(iskindof(cb, "ChildB"),true)

local tmr = moon

local ncount = 0
tmr.repeated(
	10,
	1,
	function()
		ncount = ncount + 1
		equal(ncount,1)
    end
)
local ntimes = 0
--example 一个超时10次的计时器 第二个参数是执行次数
tmr.repeated(
    10,
    10,
    function()
        ntimes = ntimes + 1
		test_assert.less_equal(ntimes,10)
    end
)

tmr.repeated(1,100,function()
	local t1 = tmr.repeated(1000,-1,function()
		test_assert.assert(false)
	end)
	tmr.remove_timer(t1)
end
)

moon.set_env("env_example","haha")
equal(moon.get_env("env_example"),"haha")

do
	local mt={}
	mt.names = "hahha"
	mt.__index = mt
	local tttt={
		a=1,
		b=2,
	}
	setmetatable(tttt,mt)
	equal(json.encode(tttt),'{"a":1,"b":2}')
end

moon.async(function()

	local co1 = moon.async(function(a,b,c,d)
		moon.co_wait(100)
		test_assert.equal(a, 1)
		test_assert.equal(b, 2)
		test_assert.equal(c, 3)
		test_assert.equal(d, 4)
	end,1,2,3,4)

	moon.co_wait(200)

	--coroutine reuse
	local co2 = moon.async(function(a,b,c,d)
		moon.co_wait(100)
		test_assert.equal(a, 5)
		test_assert.equal(b, 6)
		test_assert.equal(c, 7)
		test_assert.equal(d, 8)
	end, 5,6,7,8)

	-- create new coroutine
	local co3 = moon.async(function()
		moon.co_wait(100)
	end)

	moon.co_wait(200)

	test_assert.equal(co1, co2)
	test_assert.assert(co1~=co3 and co2~=co3)
	test_assert.equal(moon.coroutine_num(), 2)


	for _= 1, 1000 do
		moon.async(function()
			moon.co_wait(10)
		end)
	end

	moon.co_wait(100)

	--coroutine reuse
	for _= 1, 1000 do
		moon.async(function()
			moon.co_wait(10)
		end)
	end

	moon.co_wait(100)

	test_assert.equal(moon.coroutine_num(), 1000)

	test_assert.success()
end)


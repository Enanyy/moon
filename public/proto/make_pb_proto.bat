
for %%i in (*.proto) do ( 
	..\protoc --descriptor_set_out=pb/%%i %%i
)
copy pb\*.proto ..\..\server\example\pb\*.pb
pause

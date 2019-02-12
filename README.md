# DDNS Client

基于常用DDNS服务提供商的基本认证协议，实现了一个DDNS Client类库和一个可参数化配置启用的应用程序。

支持Oray、Dynu、NoIP（http get）

程序采用dotnet core2.2.1开发

以控制台方式运行，以命令行参数方式传入动态域名服务所需的参数

程序运行至少需要三个参数，最多5个参数（二个可选参数）

dotnet ./flyfire.DDNS.Client.dll yourhostname youraccount yourpassword ddnsapidomain ddnsapiuri


## 可选参数
ddnsapidomain:DDNS服务器域名

ddnsapiuri：api地址

默认为花生壳服务器相关参数

如使用dynu或noip，需传入对应参数

目前仅针对Oray进行了测试。

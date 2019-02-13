# 这是一个使用flyfire.DDNS类库的客户端

基于.net core2.2环境，支持windows和linux.

默认使用了oray的免费动态域名服务

启动时解析一次域名服务，解析域名和IP是否一致，如不一致，更新DDNS。

每十分钟检查一次，域名和公网IP不一致，则更新DDNS。

更新后会输出更新结果到控制台。

控制台输出可通过标准输出重定向输出到日志文件。

https://www.oray.com/

使用oray免费壳域名需要注册和实名认证。

## ddns使用方式

### oray 

dotnet ./flyfire.DDNS.Client.dll yourhostname yourusername yourpassword


### dynu

dotnet ./flyfire.DDNS.Client.dll yourhostname yourusername yourpassword http://api.dynu.com /nic/update


## noip

https://www.noip.com/

http://username:password@dynupdate.no-ip.com/nic/update?hostname=mytest.testdomain.com&myip=1.2.3.4

dotnet ./flyfire.DDNS.Client.dll yourhostname yourusername yourpassword http://dynupdate.no-ip.com /nic/update



## linux下脚本配置（ubuntu）

### 创建脚本文件

在程序目录下创建一个脚本文件，例如 ddns.sh

文件内容如下：

cd $(dirname $0)

pwd

nohup dotnet ./flyfire.DDNS.Client.dll yourhostname yourusername yourpassword 2>&1 >>log.txt

### 授予权限

chmod 777 ./ddns.sh

### 运行脚本

./ddns.sh

脚本将启动程序，并且程序不会随shell的关闭而退出。同时，程序输出被重定向到日志文件log.txt。

将脚本文件加入到linux的开机启动配置文件，即可实现开机自动运行。

## windows下输出重定向

dotnet ./flyfire.DDNS.Client.dll yourhostname yourusername yourpassword 2>&1 >>log.txt
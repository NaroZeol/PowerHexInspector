# PowerHexInspector

一个简单的[powertoys run](https://learn.microsoft.com/en-us/windows/powertoys/run)插件。

提供将数字转化为其他进制的功能。

[English Doc](./README_EN.md)

## 使用方法

### 触发关键词

当前触发关键词为`insp`。

### 输入格式

输入格式为
    
    insp {输入进制} {输入}

输入进制为以下的值之一

- `b`或`B`：二进制
- `d`或`D`：十进制
- `h`或`H`：十六进制

### 使用演示

![](./Images/examples/ep1.png)

点击或按下`Enter`会将转换结果复制到剪贴板中。

## 安装
下载最新的release，解压后将`PowerHexInspector`文件夹放入`%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`文件夹中，然后重启powertoys即可。

## 声明
该插件还处于开发阶段，可能会有各种未知的bug和崩溃。

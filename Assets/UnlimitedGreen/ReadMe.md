﻿# 数据检查
在UnityEditor的运行环境下将会对所有传入的参数进行数据检查，在脱离UnityEditor环境下将不会对数据进行检查。
# 数据为
为了匹配GreenLab模型，和植物模拟中常用的公式等，在该模型系统当中，**一切**、显式的可供调用时自由定义的方程中，对于将要传入所有序号性质的函数：

**一切的序号都是从1开始的，（第一个传入的年龄为1等）而不是程序中以0作为序号的起始。**

需要注意在通过Array作为汇、源函数等情况下，进行数值的转换。


# 渲染模块 Render 
## 模型数据要求
在Render当中
有几个要求
Mesh的Y轴是等于从轴到器官的朝向，
在渲染叶子的时候，叶子本地坐标的Z轴对应的是叶上方的朝向。
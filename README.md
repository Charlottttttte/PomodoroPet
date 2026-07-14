# 🍅 PomodoroPet

一个可爱的 Windows 番茄钟桌面小工具,基于 .NET 8 + WPF。

## 功能

- **标准番茄钟**:专注/休息时长可自定义,开始/暂停/重置,记录已完成的番茄数,阶段结束时播放提示音
- **迷你小屏幕模式**:点击"缩小为迷你小屏幕"后,主窗口收起,变成一个悬浮在屏幕右下角、始终置顶的像素风电子宠物小组件
  - 表情随状态变化:待机 `(-.-)`、专注 `(•ω•)`、休息 `(◕‿◕)`,还会不定期眨眼
  - 可以自由拖动到屏幕任意位置
  - 双击或点击右上角图标可展开回主界面,右键菜单可退出程序

## 运行

需要 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 或 Desktop Runtime。

```
cd PomodoroPet
dotnet run
```

或者直接编译后运行生成的 `PomodoroPet.exe`:

```
dotnet build
./bin/Debug/net8.0-windows/PomodoroPet.exe
```

## 项目结构

```
PomodoroPet/
├── Core/PomodoroEngine.cs      # 计时器核心逻辑(状态机 + DispatcherTimer)
├── MainWindow.xaml(.cs)        # 主界面
├── MiniWidgetWindow.xaml(.cs)  # 迷你悬浮小屏幕
└── Assets/icon.ico             # 应用图标
```

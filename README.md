# Example Mod 注释中文化

*NOTE: This repository is a Example Mod port but with Chinese comments, for the original version of Example Mod [click here](https://github.com/tModLoader/tModLoader).*

这是一个将Example Mod注释**汉化**的项目，用于帮助新手Mod制作者学习制作Mod，且不需要去啃原版Example Mod的生肉注释 (~~虽然有翻译软件这一回事~~)

## 我是来下载Example Mod的
直接点击绿色的 `Code` 在下拉列表中选择 `Download ZIP` 后，将文件解压到模组源码文件夹下即可 ~~不过能找到这的大概不用教这个吧~~，记得将文件夹名字改为 `ExampleMod` !

## 我想汉化Example Mod
如果你有意向向此项目作出贡献，你可以提交 `Pull request`，点击右上角的 `Fork` 按钮复刻一份仓库到你的 Github 个人仓库，然后在你的个人仓库中修改文件，随后创建 `Pull request` 即可

更多相关内容还烦请在网上自行搜索并学习 **Git** 与 **Github** 相关内容，这里不便细嗦  
(或者你开个 `Issue` 直接把你汉化好的文本+对应哪个文件发上来也行，然鹅不建议)

将这个仓库直接 `Clone` 到模组源码文件夹，运行一遍tML之后就会自动出现引用了

## 汉化建议
### 多行注释
注释一行不宜过长，在Example Mod中，注释往往被分成好几行，分行的好处是可以在一屏幕中直接看完注释，而不用来回拖条。

从该行第一个非空格字符之后算起，建议控制在**55-65个汉字**的范围内，英文则控制在**90-110个英文字母**，差不多是两倍关系

若该行以[链接](README.md)结束，则不应换行

### 斜线后加空格
为了美观，应在双斜线 `//` 后**加空格**再打注释，即:
```diff
+ // 这个是合格的注释样式
- //这个注释双斜线后没空格，不合格
```

若直接在代码行末尾添加注释，应**加空格**，即：

```diff
+ Item.DamageType = DamageClass.Ranged; // 伤害类型设置为远程，正确的注释
- Item.DamageType = DamageClass.Ranged;// 伤害类型设置为远程，错误的注释
```

### 通俗易懂
首先，机翻肯定是坏文明。

其次一些地方原Example Mod可能写得不够详尽或不够通俗，如有必要，建议自己另外写注释。

ExampleMod的翻译不需要完全按着原文来，主要是得能让人看懂。

### 注意编码类型
编码类型应一致使用 `Unicode (UTF-8 带签名)`，这里的文件大部分是从 Example Mod 直接复制过来的，编码类型往往不对。

汉化一个文件时，点击VS左上角 `文件`，在下拉列表中点击 `[文件名] 另存为...`，点击 `保存` 字样右边的小三角，选择 `编码保存`，然后选择 `Unicode (UTF-8 带签名)` 编码  
这么做了一次后，之后的编辑直接 `Ctrl+S` 保存即可

### 注意事项
- 本项目中 `Tick` 应译作 `帧` *(其在中文Wiki的叫法为“嘀嗒”，但实际上国内模组社区通常称作“帧”)*
- 本项目中 `Tile` 应译作 `物块` *(其在中文Wiki的叫法为“图格”，但实际上国内社区通常称作“物块”)*
- `Projectile` 建议译作 `射弹`
- tModLoader所作 `TODO` 备注应删去
- 注释中含有原版Wiki链接的，若有对应中文版页面，应使用**中文版**
- 原版物品、NPC、射弹名称等一律使用中文Wiki上的翻译
- 对于物品、NPC、射弹等支持名称翻译的，`SetDefault` 为英文原文，并使用 `AddTranslation` 添加中文翻译。且第一位 `GameCulture` 参数建议使用 `GameCulture.FromCultureName(GameCulture.CultureName.Chinese)`

## 这个项目目前的完成度？
0.1% (悲)  
快来汉化！

## 原版Example Mod在哪里？
tModLoader 仓库: https://github.com/tModLoader/tModLoader  
发行版: https://github.com/tModLoader/tModLoader/releases/latest

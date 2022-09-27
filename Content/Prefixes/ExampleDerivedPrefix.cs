namespace ExampleMod.Content.Prefixes
{
	// 确保你先看了“ExamplePrefix”
	//这个类展示了你可以如何用继承来简单地制作一个前缀的变种
	// 记住，继承只是上千中程序员的工具中的一种，能力越大，责任越大
	public class ExampleDerivedPrefix : ExamplePrefix // 继承自 ExamplePrefix！
	{
		// 重写 Power 属性使其返回 2.0 而不是 1.0！
		// 如果你想要修改基类实现的值，你可以写“base.Power”来引用它
		public override float Power => base.Power * 2f;

		// No need for anything else, since members get inherited from the base class.
		// 不需要其它东西，因为成员从基类继承过来了
	}
}

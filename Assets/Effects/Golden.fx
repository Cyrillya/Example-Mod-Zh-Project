sampler uTextImage : register(s0); // SpriteBatch.Draw 的内容会自动绑定到 s0
sampler uGoldenBar : register(s1); // 用于获取颜色的调色板
float uTime; // 实现调色板的滚动效果

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uTextImage, coords);

	// any 为 false 即透明色，不能改
	if (!any(color))
		return color;
	
	// 根据 uTextImage 坐标以及 uTime 的值获取在调色板上的坐标，注意要 %1.0 以确保他在 [0, 1) 区间内
    float2 barCoord = float2((coords.x + uTime) % 1.0, 0);
	
	// 在调色板上选择颜色
    return tex2D(uGoldenBar, barCoord) * color;
}

technique Technique1
{
	pass GoldenPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
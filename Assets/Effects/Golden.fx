sampler uTextImage : register(s0); // SpriteBatch.Draw �����ݻ��Զ��󶨵� s0
sampler uGoldenBar : register(s1); // ���ڻ�ȡ��ɫ�ĵ�ɫ��
float uTime; // ʵ�ֵ�ɫ��Ĺ���Ч��

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uTextImage, coords);

	// any Ϊ false ��͸��ɫ�����ܸ�
	if (!any(color))
		return color;
	
	// ���� uTextImage �����Լ� uTime ��ֵ��ȡ�ڵ�ɫ���ϵ����꣬ע��Ҫ %1.0 ��ȷ������ [0, 1) ������
    float2 barCoord = float2((coords.x + uTime) % 1.0, 0);
	
	// �ڵ�ɫ����ѡ����ɫ
    return tex2D(uGoldenBar, barCoord) * color;
}

technique Technique1
{
	pass GoldenPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
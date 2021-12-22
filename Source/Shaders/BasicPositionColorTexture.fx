struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;

};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;

};

float4x4 worldViewProj;

Texture2D shaderTexture;
SamplerState SampleType;

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;

	// Store the texture coordinates for the pixel shader.
	output.tex = input.tex;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 color = shaderTexture.Sample(SampleType, input.tex) * input.col;
	return color;
}
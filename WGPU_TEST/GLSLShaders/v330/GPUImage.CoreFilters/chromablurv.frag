#version 330 core

uniform sampler2D input_texture;

uniform vec4 pixelSize;
uniform int radiusV;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

vec4 rgb2yuv(vec4 rgb)
{
	mat4 coeffs = mat4(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

vec4 yuv2rgb(vec4 yuv)
{
	mat4 coeffs= mat4(
			 1.000, 0.000, 1.140, 0.000,
			 1.000,-0.395,-0.581, 0.000,
			 1.000, 2.032, 0.000, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
	
	return coeffs * yuv;
}

vec4 BlurYUV(vec2 uv, vec2 offset)
{
	vec4 result;
	vec4 basecol = texture(input_texture, uv);
	vec4 col = rgb2yuv(basecol);
	vec4 c = col;

	for (int i=1; i<=radiusV; i++)
		c += rgb2yuv(texture(input_texture, uv - offset*i));

	for (int i=1; i<=radiusV; i++)
		c += rgb2yuv(texture(input_texture, uv + offset*i));

	c /= (1.0 + radiusV + radiusV);
	c.x = col.x;
	result = yuv2rgb(c);
	result.a = basecol.a;

	return result;
}

void main()
{
	frag_colour = BlurYUV(v_tex, vec2(0.0, pixelSize.y));
}
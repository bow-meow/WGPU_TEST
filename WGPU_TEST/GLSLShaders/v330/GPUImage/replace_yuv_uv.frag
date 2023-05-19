#version 430 core

uniform sampler2D input_texture;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec4 RGB2YUV(vec4 rgb)
{
	mat4 coeffs= mat4(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

vec4 YUV2RGB(vec4 yuv)
{
	mat4 coeffs= mat4(
			 1.000, 0.000, 1.140, 0.000,
			 1.000,-0.395,-0.581, 0.000,
			 1.000, 2.032, 0.000, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
	
	return coeffs * yuv;
}

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
    vec4 yuv = RGB2YUV(src);
	yuv.x = 0.5;
	vec4 rgb = (YUV2RGB(yuv));
	frag_colour = vec4(rgb.xyz, 1.0);
}
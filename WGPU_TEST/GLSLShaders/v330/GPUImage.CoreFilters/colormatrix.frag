#version 330 core

uniform sampler2D input_texture;

uniform mat4 colorMatrix;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	vec4 src = texture(input_texture, v_tex);
	vec4 col = vec4(src.xyz, 1);
	vec4 col2 = col * colorMatrix;
	frag_colour = vec4(col2.xyz, src.w);
}
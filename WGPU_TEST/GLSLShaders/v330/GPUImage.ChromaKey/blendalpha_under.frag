#version 430 core

uniform sampler2D fg_texture;
uniform sampler2D bg_texture;

in VertexData2{
	vec4 uv;
} vert_out2;

out vec4 frag_colour;

void main()
{
	vec4 fg = texture(fg_texture, vert_out2.uv.xy);
	vec4 bg = texture(bg_texture, vert_out2.uv.zw);
	frag_colour.rgb = mix(bg.rgb, fg.rgb, fg.a);
	frag_colour.a = 1.0;
}
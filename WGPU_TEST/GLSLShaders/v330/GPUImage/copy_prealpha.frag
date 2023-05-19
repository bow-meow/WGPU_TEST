#version 430 core

uniform sampler2D input_texture;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 col = texture(input_texture, vert_out.uv);

	frag_colour = vec4(col.rgb * col.a, col.a);
}
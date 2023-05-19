#version 430 core

uniform sampler2D input_texture;
uniform vec2 textureScale;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	frag_colour = texture(input_texture, vert_out.uv * textureScale.xy);
}
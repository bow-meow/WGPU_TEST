#version 430 core

uniform sampler2D base_texture;
uniform sampler2D blurred_mask_texture;
uniform sampler2D background_texture;

uniform float blend;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 colour = texture(base_texture, vert_out.uv);
	
	float maskBlurred = texture(blurred_mask_texture, vert_out.uv).a;
	float mask = colour.a - maskBlurred;

	vec4 bg = texture(background_texture, vert_out.uv);

	frag_colour.rgb = mix(colour.rgb, mix(colour.rgb, bg.rgb, mask), blend);
	frag_colour.a = colour.a;
}
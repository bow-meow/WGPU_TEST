#version 430 core

uniform sampler2D input_texture;
uniform sampler2D bg_texture;

uniform float textureAspect;
uniform vec2 textureScale;
uniform float t;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
	vec2 uv = vert_out.uv - vec2(0.5, 0.5);
	float NewX = uv.x*cos(t) - uv.y*sin(t);
    float NewY = uv.x*sin(t) + uv.y*cos(t);
	uv = vec2(NewX, NewY);
	uv += vec2(0.5, 0.5);
	uv.y *= textureAspect;
	uv /= textureScale.xy;

	vec4 dst = texture(bg_texture, uv * 10);

	frag_colour.rgb = mix(src.rgb, dst.rgb, dst.a * 0.5);
	frag_colour.a = max(src.a, dst.a);
}
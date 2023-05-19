#version 330 core

uniform sampler2D input_texture;

uniform vec2 pixelSize;
uniform float strength;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void QuadSample(vec2 uvCenter, vec2 uvOffset, out vec4 c0, out vec4 c1,
		out vec4 c2, out vec4 c3, out vec4 c4)
{
    c0 = texture(input_texture, uvCenter);
	c1 = texture(input_texture, uvCenter + vec2(-uvOffset.x, uvOffset.y));
	c2 = texture(input_texture, uvCenter + vec2(uvOffset.x, uvOffset.y));
	c3 = texture(input_texture, uvCenter + vec2(uvOffset.x, -uvOffset.y));
	c4 = texture(input_texture, uvCenter - vec2(uvOffset.x, uvOffset.y));
}

void main()
{
	vec4 c0, c1, c2, c3, c4;
	QuadSample(v_tex, pixelSize.xy * 1, c0, c1, c2, c3, c4);
	
	frag_colour.a = mix(c0.a, max(max(c1.a, c2.a), max(c3.a, c4.a)), strength);
	frag_colour.rgb = c0.rgb;
}
#version 330 core

uniform sampler2D input_texture;

uniform vec3 rGain;
uniform vec3 gGain;
uniform vec3 bGain;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	vec4 col = texture(input_texture, v_tex);

	float rOut = dot(rGain.rgb, col.rgb);
	float gOut = dot(gGain.rgb, col.rgb);
	float bOut = dot(bGain.rgb, col.rgb);
	
	frag_colour = vec4(rOut, gOut, bOut, col.a);
}
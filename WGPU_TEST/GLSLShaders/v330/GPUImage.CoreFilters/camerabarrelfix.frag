#version 430 core

uniform sampler2D input_texture;
uniform float focal_length;
uniform float principal_x;
uniform float principal_y;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	float k1 = 0.4142;    // constant for radial distortion correction
	float k2 = 0.40348;
	vec2 xy = (vert_out.uv - vec2(principal_x, principal_y))/vec2(focal_length,focal_length);

	float r = sqrt( dot(xy, xy) );
	float r2 = r * r;
	float r4 = r2 * r2;
	float coeff = (k1 * r2 + k2 * r4);

	// add the calculated offsets to the current texture coordinates
	xy = ((xy + xy * coeff.xx) * focal_length.xx) + vec2(principal_x, principal_y);

	// look up the texture at the corrected coordinates
	// and output the color
	frag_colour = texture(input_texture, xy);
}
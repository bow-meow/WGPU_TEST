#version 430 core

uniform sampler2D input_texture;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

vec3 RGB2HSV(float r, float g, float b)
{
	float themin,themax,delta;
	vec3 c2;

	themin = min(r,min(g,b));
	themax = max(r,max(g,b));
	delta = themax - themin;
	c2.z = themax;			// value
	c2.y = 0.0f;				// saturation
	if (themax > 0.0f)
		c2.y = delta / themax;
	c2.x = 0.0f;				// hue
	if (delta > 0.0f)
	{
		if (themax == r && themax != g)
			c2.x += (g - b) / delta;
		if (themax == g && themax != b)
			c2.x += (2.0f + (b - r) / delta);
		if (themax == b && themax != r)
			c2.x += (4.0f + (r - g) / delta);
		c2.x *= 60.0f;
	}
	return c2;
}

vec3 HSV2RGB(vec3 HSV)
{
    vec3 RGB = vec3(HSV.z, HSV.z, HSV.z);
    if ( HSV.y != 0 ) {
       float var_h = HSV.x * 6;
       float var_i = floor(var_h);
       float var_1 = HSV.z * (1.0 - HSV.y);
       float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
       float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
       if      (var_i == 0) { RGB = vec3(HSV.z, var_3, var_1); }
       else if (var_i == 1) { RGB = vec3(var_2, HSV.z, var_1); }
       else if (var_i == 2) { RGB = vec3(var_1, HSV.z, var_3); }
       else if (var_i == 3) { RGB = vec3(var_1, var_2, HSV.z); }
       else if (var_i == 4) { RGB = vec3(var_3, var_1, HSV.z); }
       else                 { RGB = vec3(HSV.z, var_1, var_2); }
   }
   return (RGB);
}

void main()
{
	vec4 col = texture(input_texture, v_tex);
	vec3 hsv = RGB2HSV(col.x, col.y, col.z);

	hsv.x /= 360.0;
    hsv.y = 1.0;
    hsv.z = 1.0;
    vec3 rgb = HSV2RGB(hsv);
	frag_colour = vec4(rgb, 1.0);
}
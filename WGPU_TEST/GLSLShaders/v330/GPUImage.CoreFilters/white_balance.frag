#version 440 core

uniform sampler2D input_texture;

uniform float red;
uniform float green;
uniform float blue;
uniform float strength;
uniform float keepwhite;
uniform vec3 ref_d65;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;


vec3 RGB2HSV(vec3 color)
{
	float r = color.r;
	float g = color.g;
	float b = color.b;
	float themin,themax,delta;
	vec3 c2;

	themin = min(r,min(g,b));
	themax = max(r,max(g,b));
	delta = themax - themin;
	c2.z = themax;			// value
	c2.y = 0.0;				// saturation
	if (themax > 0.0)
		c2.y = delta / themax;
	c2.x = 0.0;				// hue
	if (delta > 0.0)
	{
		if (themax == r && themax != g)
			c2.x += (g - b) / delta;
		if (themax == g && themax != b)
			c2.x += (2.0 + (b - r) / delta);
		if (themax == b && themax != r)
			c2.x += (4.0 + (r - g) / delta);
		c2.x *= 60.0;
	}
	return c2;
}

vec3 xyz2lab(vec3 xyz)
{
	xyz /= ref_d65;

	if ( xyz.x > 0.008856 )
		xyz.x = pow(xyz.x, ( 1.0/3.0 ));
	else
		xyz.x = ( 7.787 * xyz.x ) + ( 16.0 / 116.0 );

	if ( xyz.y > 0.008856 )
		xyz.y = pow(xyz.y, ( 1.0/3.0 ));
	else
		xyz.y = ( 7.787 * xyz.y ) + ( 16.0 / 116.0 );

	if ( xyz.z > 0.008856 )
		xyz.z = pow(xyz.z, ( 1.0/3.0 ));
	else
		xyz.z = ( 7.787 * xyz.z ) + ( 16.0 / 116.0 );

	float l = ( 116.0 * xyz.y ) - 16.0;
	float a = 500.0 * ( xyz.x - xyz.y );
	float b = 200.0 * ( xyz.y - xyz.z );

	return vec3(l, a, b);
}

vec3 rgb2xyz(vec3 rgb)
{
	if (rgb.r > 0.04045)
		rgb.r = pow( (( rgb.r + 0.055 ) / 1.055 ), 2.4);
	else
		rgb.r = rgb.r / 12.92;

	if (rgb.g > 0.04045)
		rgb.g = pow( (( rgb.g + 0.055 ) / 1.055 ), 2.4);
	else
		rgb.g = rgb.g / 12.92;

	if (rgb.b > 0.04045)
		rgb.b = pow( (( rgb.b + 0.055 ) / 1.055 ), 2.4);
	else
		rgb.b = rgb.b / 12.92;

	rgb *= 100.0;

	float x = rgb.r * 0.4124 + rgb.g * 0.3576 + rgb.b * 0.1805;
	float y = rgb.r * 0.2126 + rgb.g * 0.7152 + rgb.b * 0.0722;
	float z = rgb.r * 0.0193 + rgb.g * 0.1192 + rgb.b * 0.9505;

	return vec3(x, y, z);
}

vec3 rgb2lab(vec3 rgb)
{
	vec3 xyz = rgb2xyz(rgb);
	return xyz2lab(xyz);
}

float labclamp(float x)
{
	if (pow( x,3.0) > 0.008856 ) 
		x = pow(x, 3.0);
	else              
		x = ( x - 16.0 / 116.0 ) / 7.787;
	return x;
}

vec3 lab2xyz(vec3 lab)
{
	float l = lab.x;
	float a = lab.y;
	float b = lab.z;

	float y = ( l + 16.0 ) / 116.0;
	float x = a / 500.0 + y;
	float z = y - b / 200.0;

	y = labclamp(y);
	x = labclamp(x);
	z = labclamp(z);

	return vec3(x, y, z) * ref_d65;
}

vec3 xyz2rgb(vec3 xyz)
{
	xyz /= 100.0;;

	float r = xyz.x *  3.2406 + xyz.y * -1.5372 + xyz.z * -0.4986;
	float g = xyz.x * -0.9689 + xyz.y *  1.8758 + xyz.z *  0.0415;
	float b = xyz.x *  0.0557 + xyz.y * -0.2040 + xyz.z *  1.0570;

	if ( r > 0.0031308 )
		r = 1.055 * pow( r , ( 1.0 / 2.4 ) ) - 0.055;
	else
		r = 12.92 * r;

	if ( g > 0.0031308 )
		g = 1.055 * pow( g , ( 1.0 / 2.4 ) ) - 0.055;
	else
		g = 12.92 * g;

	if ( b > 0.0031308 )
		b = 1.055 * pow( b , ( 1.0 / 2.4 ) ) - 0.055;
	else
		b = 12.92 * b;

	return vec3(r, g, b);
}


vec3 lab2rgb(vec3 lab)
{
	vec3 xyz = lab2xyz(lab);
	return xyz2rgb(xyz);
}

void main()
{
	vec4 rgb = texture(input_texture, v_tex);//vert_out.uv);

	vec3 hsv = RGB2HSV(rgb.rgb);
	vec3 lab = rgb2lab(rgb.rgb);
	vec3 delta = rgb2lab(vec3(red, green, blue));

	// scale delta by amount of luma
	delta.y = delta.y * (lab.x / 100.0) * 1.1;
	delta.z = delta.z * (lab.x / 100.0) * 1.1;

	lab.yz -= delta.yz;

	vec4 result = rgb;
	result.xyz = clamp(lab2rgb(lab), 0.0, 1.0);

	// very has less effect on pixels that are both very desaturated and bright (whites/grays)
	/*
		0.9 0.1		saturated, dark (dark colour)	 = 1
		0.9 0.5     saturated, medium (medium col)   = 1
		0.9 0.9		saturated, bright (colour)		 = 1
		0.1 0.1		unsat, dark (dark grey/white)	 = 0
		0.1 0.5     unsat, medium (medium grey)      = 0.5
		0.1 0.9		unsat bright (white)			 = 0
	1..0..1
	0..1..0

	*/
	float valfac = abs((hsv.z - 0.5) * 2.0);
	valfac = pow(valfac, 12.0) * (1.0-hsv.y);		// to make sure we only get the very darks and very brights
	valfac = mix(1.0-valfac, 1.0, hsv.y);// * (1-valfac));

	result = mix(result, rgb, (1.0-valfac) * keepwhite);

	frag_colour = clamp(mix(rgb, result, strength * 1.5), 0.0, 1.0);
}
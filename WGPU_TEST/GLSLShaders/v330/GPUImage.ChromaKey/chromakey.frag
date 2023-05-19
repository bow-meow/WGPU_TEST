#version 430 core

uniform sampler2D input_texture;

uniform vec3 keyHSV;
uniform float edgeFeather;
uniform float edgeDistance;
uniform float spillEdgeFeather;
uniform float spillEdgeDistance;
uniform float spillLuma;
uniform float spillAmount;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

layout (location = 0) out vec4 frag_colour;

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

float CircularDistance(float a, float b, float range)
{
	float t = sign (a-b);
	float d = min(abs(a-b), range - abs(a-b));
	d /= 180.0;
	d = clamp(d, 0, 1);
	if (t < 0)
		d = 1-d;
	return d;
	
}

float GetHueDistance(float hue1, float hue2)
{
	return CircularDistance(hue1, hue2, 360);
}

float GetHSVDistance(vec3 hsv1, vec3 hsv2)
{
	float hue1 = radians(hsv1.x);
	float hue2 = radians(hsv2.x);
	
	vec2 hueA = vec2(cos(hue1), sin(hue1));
	vec2 hueB = vec2(cos(hue2), sin(hue2));
	
	float hueDist = length(hueA - hueB) * 0.5;
	float valDist = abs(hsv1.z - hsv2.z);
	float satDist = abs(hsv1.y - hsv2.y);

  	return (hueDist + valDist + satDist) / 3.0;
}

float getFraction(float distance, float q, float value)
{
	value -= distance;
	value = value / q;
	value = clamp(value, 0, 1);

	return value;
}

vec3 hsv_to_rgb(vec3 HSV)
{
    vec3 RGB = HSV.zzz;
    if ( HSV.y != 0 ) {
       float var_h = HSV.x * 6;
       float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
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


vec3 AntiSpill(vec4 col, vec3 sourceHSV, vec3 key)
{
	float hueDistance = GetHueDistance(sourceHSV.x, key.x);

	hueDistance = clamp((hueDistance - spillEdgeDistance) / (spillEdgeFeather), 0, 1);

	float target = dot(col.rgb, vec3(0.299, 0.587, 0.114));
	vec3 targetCol = vec3(target, target, target);
	vec3 icol = 1-col.rgb;
	targetCol = hsv_to_rgb(vec3(sourceHSV.x-180, sourceHSV.y, clamp(sourceHSV.z - spillLuma, 0, 1)));

	return mix(col.rgb, targetCol, (clamp((1.0 - hueDistance) - spillAmount, 0, 1) ));
}

vec4 ChromaKeyHSV(vec4 col)
{
	// calculate distance from key color
	vec3 sourceHSV = RGB2HSV(col.rgb);
	
	float colorDistance = GetHSVDistance(keyHSV, sourceHSV);

	// create mask value, with feathering
    float f = getFraction(edgeDistance, edgeFeather, colorDistance);
	col.a *= f;
	
	col.rgb = AntiSpill(col, sourceHSV, keyHSV);
	
	return col;
}

void main()
{
	vec4 col = texture(input_texture, v_tex);
	frag_colour = ChromaKeyHSV(col);
	
	frag_colour.rgb *= frag_colour.a;
}
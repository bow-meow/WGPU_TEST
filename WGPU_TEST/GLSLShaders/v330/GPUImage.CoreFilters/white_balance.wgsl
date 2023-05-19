struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	red: f32,
	green: f32,
	blue: f32,
	strength: f32,
	keep_white: f32,
	ref_d65: vec3<f32>
};

fn RGB2HSV(color: vec3<f32>) -> vec3<f32>
{
	let r: f32 = color.r;
	let g: f32 = color.g;
	let b: f32 = color.b;

	let the_min: f32 = min(r, min(g, b));
	let the_max: f32 = max(r, max(g, b));
	let delta: f32 = the_max - the_min;

	var c2: vec3<f32>;

	c2.z = the_max;			// value
	c2.y = 0.0;				// saturation
	if (the_max > 0.0){
		c2.y = delta / the_max;
	}
	c2.x = 0.0;				// hue
	if (delta > 0.0){
		if (the_max == r && the_max != g){
			c2.x += (g - b) / delta;
		}
		if (the_max == g && the_max != b){
			c2.x += (2.0 + (b - r) / delta);
		}
		if (the_max == b && the_max != r){
			c2.x += (4.0 + (r - g) / delta);
		}
		c2.x *= 60.0;
	}
	return c2;
}

fn xyz2lab(xyz: vec3<f32>) -> vec3<f32>
{
	var var_xyz = vec3<f32>(xyz);

	var_xyz /= data.ref_d65;

	if (var_xyz.x > 0.008856){
		var_xyz.x = pow(var_xyz.x, (1.0 / 3.0));
	}	
	else{
		var_xyz.x = (7.787 * var_xyz.x) + (16.0 / 116.0);
	}
		
	if (var_xyz.y > 0.008856){
		var_xyz.y = pow(var_xyz.y, (1.0 / 3.0));
	}
	else{
		var_xyz.y = (7.787 * var_xyz.y) + (16.0 / 116.0);
	}

	if (var_xyz.z > 0.008856){
		var_xyz.z = pow(var_xyz.z, (1.0 / 3.0));
	}	
	else{
		var_xyz.z = (7.787 * var_xyz.z) + (16.0 / 116.0);
	}
		
	let l: f32 = (116.0 * var_xyz.y) - 16.0;
	let a: f32 = 500.0 * (var_xyz.x - var_xyz.y);
	let b: f32 = 200.0 * (var_xyz.y - var_xyz.z);

	return vec3<f32>(l, a, b);
}

fn rgb2xyz(rgb: vec3<f32>) -> vec3<f32>
{
	var var_rgb = vec3<f32>(rgb);

	if (var_rgb.r > 0.04045){
		var_rgb.r = pow( ((var_rgb.r + 0.055 ) / 1.055 ), 2.4);
	}	
	else{
		var_rgb.r = var_rgb.r / 12.92;
	}
		
	if (var_rgb.g > 0.04045){
		var_rgb.g = pow(((var_rgb.g + 0.055 ) / 1.055 ), 2.4);
	}
	else{
		var_rgb.g = var_rgb.g / 12.92;
	}
		
	if (var_rgb.b > 0.04045){
		var_rgb.b = pow(((var_rgb.b + 0.055 ) / 1.055 ), 2.4);
	}
	else{
		var_rgb.b = var_rgb.b / 12.92;
	}
		
	var_rgb *= 100.0;

	let x: f32 = var_rgb.r * 0.4124 + var_rgb.g * 0.3576 + var_rgb.b * 0.1805;
	let y: f32 = var_rgb.r * 0.2126 + var_rgb.g * 0.7152 + var_rgb.b * 0.0722;
	let z: f32 = var_rgb.r * 0.0193 + var_rgb.g * 0.1192 + var_rgb.b * 0.9505;

	return vec3<f32>(x, y, z);
}

fn rgb2lab(rgb: vec3<f32>) -> vec3<f32>
{
	let xyz: vec3<f32> = rgb2xyz(rgb);
	return xyz2lab(xyz);
}

fn labclamp(x: f32) -> f32
{
	var var_x = x;

	if (pow(var_x, 3.0) > 0.008856){
		var_x = pow(var_x, 3.0);
	}
	else{
		var_x = (var_x - 16.0 / 116.0) / 7.787;
	}            
	return var_x;
}

fn lab2xyz(lab: vec3<f32>) -> vec3<f32>
{
	let l: f32 = lab.x;
	let a: f32 = lab.y;
	let b: f32 = lab.z;

	var y: f32 = (l + 16.0) / 116.0;
	var x: f32 = a / 500.0 + y;
	var z: f32 = y - b / 200.0;

	y = labclamp(y);
	x = labclamp(x);
	z = labclamp(z);

	return vec3<f32>(x, y, z) * data.ref_d65;
}

fn xyz2rgb(xyz: vec3<f32>) -> vec3<f32>
{
	var var_xyz = vec3<f32>(xyz);

	var_xyz /= 100.0;

	var r: f32 = var_xyz.x *  3.2406 + var_xyz.y * -1.5372 + var_xyz.z * -0.4986;
	var g: f32 = var_xyz.x * -0.9689 + var_xyz.y *  1.8758 + var_xyz.z *  0.0415;
	var b: f32 = var_xyz.x *  0.0557 + var_xyz.y * -0.2040 + var_xyz.z *  1.0570;

	if (r > 0.0031308){
		r = 1.055 * pow(r , (1.0 / 2.4)) - 0.055;
	}
	else{
		r = 12.92 * r;
	}
		
	if (g > 0.0031308){
		g = 1.055 * pow(g , (1.0 / 2.4 )) - 0.055;
	}	
	else{
		g = 12.92 * g;
	}

	if (b > 0.0031308){
		b = 1.055 * pow(b , (1.0 / 2.4)) - 0.055;
	}
	else{
		b = 12.92 * b;
	}
		
	return vec3<f32>(r, g, b);
}

fn lab2rgb(lab: vec3<f32>) -> vec3<f32>
{
	let xyz: vec3<f32> = lab2xyz(lab);
	return xyz2rgb(xyz);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let rgb: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);

	let hsv: vec3<f32> = RGB2HSV(rgb.rgb);
	var lab: vec3<f32> = rgb2lab(rgb.rgb);
	var delta: vec3<f32> = rgb2lab(vec3<f32>(data.red, data.green, data.blue));
	
	// scale delta by amount of luma
	delta.y = delta.y * (lab.x / 100.0) * 1.1;
	delta.z = delta.z * (lab.x / 100.0) * 1.1;
	
	lab.y -= delta.y;
	lab.z -= delta.z;
	
	var result = rgb;

	let c: vec3<f32> = clamp(lab2rgb(lab), vec3<f32>(0.0), vec3<f32>(1.0));
	result.x = c.x;
	result.y = c.y;
	result.z = c.z;
	
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
	var valfac: f32 = abs((hsv.z - 0.5) * 2.0);
	valfac = pow(valfac, 12.0) * (1.0 - hsv.y);		// to make sure we only get the very darks and very brights
	valfac = mix(1.0 - valfac, 1.0, hsv.y);// * (1 - valfac));
	
	result = mix(result, rgb, (1.0 - valfac) * data.keep_white);
	
	return clamp(mix(rgb, result, data.strength * 1.5), vec4<f32>(0.0), vec4<f32>(1.0));
}
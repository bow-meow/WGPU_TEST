struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	key_hsv: vec3<f32>,
	hue_distance: f32,
	sat_distance: f32,
	val_distance: f32,
	hue_feather: f32,
	sat_feather: f32,
	val_feather: f32,
	hue_feather2: f32,
	is_alpha_premultiplied: i32,
};

fn RGB2HSV(color: vec3<f32>) -> vec3<f32>
{
	let r: f32 = color.r;
	let g: f32 = color.g;
	let b: f32 = color.b;
	
	let the_min = min(r, min(g, b));
	let the_max = max(r, max(g, b));
	let delta = the_max - the_min;
	
	var c2: vec3<f32>;
	
	c2.z = the_max;				// value
	c2.y = 0.0;					// saturation
	if (the_max > 0.0){
		c2.y = delta / the_max;
	}
	c2.x = 0.0;					// hue
	if (delta > 0.0)
	{
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

fn CircularDistance(a: f32, b: f32, range: f32) -> f32
{
	return min(abs(a - b), range - abs(a - b));
}

fn GetHueDistance(hue1: f32, hue2: f32) -> f32
{
	return CircularDistance(hue1, hue2, 360.0) / 180.0;
}

fn GetSafeHueDistance(hue1: f32, hue2: f32) -> f32
{
	var var_hue1 = radians(hue1);
	var var_hue2 = radians(hue2);

	// Circular distance
	let a = vec2<f32>(cos(var_hue1), sin(var_hue1));
	let b = vec2<f32>(cos(var_hue2), sin(var_hue2));
	
	return length(a - b) * 0.5;
}

fn SignedCircularDistance(a: f32, b: f32, range: f32) -> f32
{
	// non-wrapped abs distance
	let d1: f32 = abs(a - b);
		
	// wrapped abs distance	
	let d2: f32 = range - d1;

	var d: f32 = min(d1, d2);
	if (d2 < d1){
		d *= sign(a - b);
	}else{
		d *= sign(b - a);
	}
	return d;
}

fn GetSignedHueDistance(hue1: f32, hue2: f32) -> f32
{
	return SignedCircularDistance(hue1, hue2, 360.0) / 180.0;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>{
	var src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);

	if(data.is_alpha_premultiplied == 1){
		src = vec4<f32>(src.r / src.a, src.g / src.a ,src.b / src.a, src.a);
	}
	let hsv: vec3<f32> = RGB2HSV(src.rgb);
	
	let hue_dist:  f32 = GetHueDistance(data.key_hsv.x, hsv.x);
	let hue_dist2: f32 = GetSafeHueDistance(data.key_hsv.x, hsv.x);
	let hue_dist3: f32 = GetSignedHueDistance(data.key_hsv.x, hsv.x);
	
	
	let values = vec3<f32>(abs(hue_dist3), hsv.y, hsv.z);
	let start_dists = vec3<f32>(data.hue_distance, data.sat_distance, data.val_distance);
	var feathers = vec3<f32>(data.hue_feather, data.sat_feather, data.val_feather);
	if (hue_dist3 < 0.0){
		feathers.x = data.hue_feather2;
	}
	let end_dists: vec3<f32> = start_dists + feathers;
	let a: vec3<f32> = smoothstep(start_dists, end_dists, values);
	
	
	let alpha: f32 = clamp(length(vec3<f32>(a.x, 1.0 - a.y, 1.0 - a.z)), 0.0, 1.0);
	
	src.a = min(alpha, src.a);
	
	if(data.is_alpha_premultiplied == 1){
		src = vec4(src.r * alpha, src.g * alpha, src.b * alpha, alpha);
	}
	return src;
}          
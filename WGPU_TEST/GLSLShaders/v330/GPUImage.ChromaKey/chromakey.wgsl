struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	key_hsv: vec3<f32>,
	edge_feather: f32,
	edge_distance: f32,
	spill_edge_feather: f32,
	spill_edge_distance: f32,
	spill_luma: f32,
	spill_amount: f32
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
	let t = sign(a - b);
	var d = min(abs(a - b), range - abs(a - b));
	d /= 180.0;
	d = clamp(d, 0.0, 1.0);
	if (t < 0.0){
		d = 1.0 - d;
	}
	return d;
}

fn GetHueDistance(hue1: f32, hue2: f32) -> f32
{
	return CircularDistance(hue1, hue2, 360.0);
}

fn GetHSVDistance(hsv1: vec3<f32>, hsv2: vec3<f32>) -> f32
{
	let hue1 = radians(hsv1.x);
	let hue2 = radians(hsv2.x);
	
	let hueA = vec2<f32>(cos(hue1), sin(hue1));
	let hueB = vec2<f32>(cos(hue2), sin(hue2));
	
	let hueDist = length(hueA - hueB) * 0.5;
	let valDist = abs(hsv1.z - hsv2.z);
	let satDist = abs(hsv1.y - hsv2.y);

  	return (hueDist + valDist + satDist) / 3.0;
}

fn getFraction(distance: f32, q: f32, value: f32) -> f32
{
	var var_value: f32 = value;
	var_value -= distance;
	var_value = var_value / q;
	var_value = clamp(var_value, 0.0, 1.0);

	return var_value;
}

fn hsv_to_rgb(hsv: vec3<f32>) -> vec3<f32>
{
    var rgb: vec3<f32> = hsv.zzz;
    if (hsv.y != 0.0) {
       let var_h: f32 = hsv.x * 6.0;
       let var_i: f32 = floor(var_h);   // Or ... var_i = floor( var_h )
       let var_1: f32 = hsv.z * (1.0 - hsv.y);
       let var_2: f32 = hsv.z * (1.0 - hsv.y * (var_h - var_i));
       let var_3: f32 = hsv.z * (1.0 - hsv.y * (1.0 - (var_h - var_i)));
       if      (var_i == 0.0) { rgb = vec3<f32>(hsv.z, var_3, var_1); }
       else if (var_i == 1.0) { rgb = vec3<f32>(var_2, hsv.z, var_1); }
       else if (var_i == 2.0) { rgb = vec3<f32>(var_1, hsv.z, var_3); }
       else if (var_i == 3.0) { rgb = vec3<f32>(var_1, var_2, hsv.z); }
       else if (var_i == 4.0) { rgb = vec3<f32>(var_3, var_1, hsv.z); }
       else					  { rgb = vec3<f32>(hsv.z, var_1, var_2); }
   }
   return (rgb);
}

fn AntiSpill(col: vec4<f32>, source_hsv: vec3<f32>, key: vec3<f32>) -> vec3<f32>
{
	var hueDistance: f32 = GetHueDistance(source_hsv.x, key.x);

	hueDistance = clamp((hueDistance - data.spill_edge_distance) / (data.spill_edge_feather), 0.0, 1.0);

	let target_val: f32 = dot(col.rgb, vec3<f32>(0.299, 0.587, 0.114));
	var target_col = vec3<f32>(target_val, target_val, target_val);
	//not used: vec3 icol = 1.0 - col.rgb;
	target_col = hsv_to_rgb(vec3<f32>(source_hsv.x - 180.0, source_hsv.y, clamp(source_hsv.z - data.spill_luma, 0.0, 1.0)));

	return mix(col.rgb, target_col, (clamp((1.0 - hueDistance) - data.spill_amount, 0.0, 1.0) ));
}

fn ChromaKeyHSV(col: vec4<f32>) -> vec4<f32>
{
	var var_col: vec4<f32> = col;

	// calculate distance from key color
	let source_hsv: vec3<f32> = RGB2HSV(var_col.rgb);
	
	let color_distance: f32 = GetHSVDistance(data.key_hsv, source_hsv);

	// create mask value, with feathering
    let f: f32 = getFraction(data.edge_distance, data.edge_feather, color_distance);
	var_col.a *= f;
	
	let anti_spill_color: vec3<f32> = AntiSpill(var_col, source_hsv, data.key_hsv);

	return vec4<f32>(anti_spill_color, var_col.a);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	var color: vec4<f32> = ChromaKeyHSV(src);
	
	color.r *= color.a;
	color.g *= color.a;
	color.b *= color.a;

	return color;
}
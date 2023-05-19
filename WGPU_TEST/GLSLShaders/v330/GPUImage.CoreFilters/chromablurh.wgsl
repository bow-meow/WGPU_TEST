struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>,
};

// Fragment shader

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	pixel_size: vec4<f32>,
	radius_h: i32
};

fn rgb2yuv(rgb: vec4<f32>) -> vec4<f32>
{
	let coeffs: mat4x4<f32> = mat4x4<f32>(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

fn yuv2rgb(yuv: vec4<f32>) -> vec4<f32>
{
	let coeffs: mat4x4<f32> = mat4x4<f32>(
			 1.000, 0.000, 1.140, 0.000,
			 1.000,-0.395,-0.581, 0.000,
			 1.000, 2.032, 0.000, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
	
	return coeffs * yuv;
}

fn BlurYUV(tex_coords: vec2<f32>, offset: vec2<f32>) -> vec4<f32>
{
	var result: vec4<f32>;
	let src: vec4<f32> = textureSample(input_tex, input_sampler, tex_coords);
	let col: vec4<f32> = rgb2yuv(src);
	var c: vec4<f32> = col;

	for (var i = 1; i <= data.radius_h; i++){
		let rgb2yuv_result: vec4<f32> = rgb2yuv(textureSample(input_tex, input_sampler, tex_coords - offset * f32(i)));
		c += rgb2yuv_result;
	}

	for (var i = 1; i <= data.radius_h; i++){
		let rgb2yuv_result: vec4<f32> = rgb2yuv(textureSample(input_tex, input_sampler, tex_coords + offset * f32(i)));
		c += rgb2yuv_result;
	}

	let radius_h_f32 = f32(data.radius_h);

	c /= (1.0 + radius_h_f32 + radius_h_f32);
	c.x = col.x;
	result = yuv2rgb(c);
	result.a = src.a;

	return result;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>{

	return BlurYUV(in.tex_coords, vec2<f32>(data.pixel_size.x, 0.0));
}
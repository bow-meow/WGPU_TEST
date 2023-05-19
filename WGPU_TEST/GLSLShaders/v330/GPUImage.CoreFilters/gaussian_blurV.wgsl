struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	pixel_size: vec4<f32>,
	radius: f32
};

fn Blur(uv: vec2<f32>, offset: vec2<f32>) -> vec4<f32>
{
	var result: vec4<f32>;
	let col = textureSample(input_tex, input_sampler, uv);
	var c = col;

	let radius_i32 = i32(data.radius);

	for (var i = 1.0; i <= data.radius; i += 1.0)
	{
		let textureSample_result: vec4<f32> = textureSample(input_tex, input_sampler, uv - offset * i);
		c += textureSample_result;
	}

	for (var i = 1.0; i <= data.radius; i += 1.0)
	{
		let textureSample_result: vec4<f32> = textureSample(input_tex, input_sampler, uv + offset * i);
		c += textureSample_result;
	}

	c /= (1.0 + data.radius + data.radius);
	result = c;

	return result;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	return Blur(in.tex_coords, vec2<f32>(0.0, data.pixel_size.y * 1.5));
}
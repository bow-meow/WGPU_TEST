struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	pixel_size: vec4<f32>,
	factor: f32
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let c0: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	var c: vec4<f32> = c0 * 13.0;

	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(data.pixel_size.x, 0.0)) * -2.0;
	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(-data.pixel_size.x, 0.0)) * -2.0;
	
	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(data.pixel_size.x, -data.pixel_size.y)) * -1.0;
	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(0.0, -data.pixel_size.y)) * -2.0;
	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(-data.pixel_size.x, -data.pixel_size.y)) * -1.0;

	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(data.pixel_size.x, data.pixel_size.y)) * -1.0;
	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(0.0, data.pixel_size.y)) * -2.0;
	c += textureSample(input_tex, input_sampler, in.tex_coords + vec2<f32>(-data.pixel_size.x, data.pixel_size.y)) * -1.0;
	
	return mix(c0, c, data.factor);
}
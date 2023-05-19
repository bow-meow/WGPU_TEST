struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	texture_scale: vec2<f32>
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	return textureSample(input_tex, input_sampler, in.tex_coords * data.texture_scale.xy);
}
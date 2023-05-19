struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src = textureSample(input_tex, input_sampler, in.tex_coords);

	return vec4<f32>(src.rgb * src.a, src.a);
}
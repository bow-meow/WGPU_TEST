struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var fg_tex: texture_2d<f32>;
@group(0) @binding(1) var bg_tex: texture_2d<f32>;
@group(0) @binding(2) var input_sampler: sampler;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let fg: vec4<f32> = textureSample(fg_tex, input_sampler, in.tex_coords.xy);
	let bg: vec4<f32> = textureSample(bg_tex, input_sampler, in.tex_coords.xy);
	var color = mix(bg, fg, fg.a);
	color.a = 1.0;
	return color;
}
struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

var<private> white_tolerance: f32;

struct DataUniform{
	tolerance: f32,
	new_black: vec4<f32>,
	new_white: vec4<f32>,
};

fn RGB2HighlightRGB(color: vec4<f32>) -> vec4<f32>
{
	var result: vec4<f32> = color;
	if (color.r <= data.tolerance && color.g <= data.tolerance && color.b <= data.tolerance)
		{
			result = data.new_black;
			result.a = color.a;
		}
	
	if(color.r >= white_tolerance && color.g >= white_tolerance && color.b >= white_tolerance)
		{
			result = data.new_white;
			result.a = color.a;
		}
	
	return result;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	white_tolerance = 1.0 - data.tolerance;
	return RGB2HighlightRGB(textureSample(input_tex, input_sampler, in.tex_coords));
}

// Vertex shader
struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

// Fragment shader

struct DataUniform{
    color: vec4<f32>,
    color_mask: vec4<f32>,
    top: f32,
    left: f32,
    bottom: f32,
    right: f32,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32> {
    let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);

	if (in.tex_coords.y < data.top || in.tex_coords.y > data.bottom || in.tex_coords.x < data.left || in.tex_coords.x > data.right){
    	return data.color;
    } else{
        return src;
    }
}
 
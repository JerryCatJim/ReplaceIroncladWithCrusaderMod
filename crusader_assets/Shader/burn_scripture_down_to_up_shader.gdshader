shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

uniform sampler2D col_texture  : source_color, filter_linear_mipmap, repeat_enable;
uniform sampler2D ink_texture  : source_color, filter_linear_mipmap, repeat_enable;
uniform sampler2D noise_texture : hint_default_black, filter_linear_mipmap, repeat_enable;
uniform sampler2D burn_texture : source_color, filter_linear_mipmap; // GradientTexture1D

// Rim（与 StandardMaterial3D 对齐）
uniform float rim_amount     : hint_range(0.0, 1.0) = 1.0;
uniform float rim_tint_value : hint_range(0.0, 1.0) = 0.0;
// 溶解 / 燃烧
uniform float dissolve_value : hint_range(0.0, 1.0) = 1.0; // 1=完整 0=消失
uniform float burn_size      : hint_range(0.0, 10.0) = 0.3;
uniform float burn_intensity : hint_range(0.0, 20.0) = 5.0;
// 金色整体发光（径向衰减）
uniform vec4  glow_color        : source_color = vec4(1.0, 0.85, 0.25, 1.0);
uniform float brightness        : hint_range(0.0, 20.0) = 1.0;
uniform float brightness_radius : hint_range(0.1, 2.0) = 1.0;
uniform vec3  glow_center       = vec3(0.5, 2.0, 0.0);
//单向燃烧相关
uniform vec3  burn_axis   = vec3(-0.5, 0.0, 1.0);  // 燃烧推进的方向（按你模型的长轴填）
uniform float model_length = 1.2;                 // 沿该轴的实际长度
uniform float model_offset = 0.65;                 // 该轴起点相对原点的偏移

varying vec3 local_pos;
float inverse_lerp(float a, float b, float v) {
    return (v - a) / (b - a);
}

void vertex() {
    local_pos = VERTEX;
}

void fragment() {
    float dv = max(dissolve_value, 0.001);
    vec4  col_sample = texture(col_texture, UV);
    vec4  ink_sample = texture(ink_texture, UV);
	float noise_raw = texture(noise_texture, UV).r;
    //float noise      = texture(noise_texture, UV).r * (1.0 - UV.y);;   // 从下往上烧
	float proj = dot(local_pos, normalize(burn_axis));
	float h = 1.0-clamp((proj - model_offset) / model_length, 0.0, 1.0);
	float noise = noise_raw * 0.3 + h * 0.7;
	// 三区划分
    float keep      = step(noise, dv);
    float burn_band = step(noise, dv * (burn_size+1.0));
    float burn_mask = burn_band * (1.0 - keep);
    // 渐变图采样
    vec2 burn_uv = vec2(inverse_lerp(dv, dv * (burn_size+1.0), noise), 0.0);
    vec4 burn = texture(burn_texture, burn_uv);
    // 基础色：未烧区显示原色，燃烧带显示渐变
    ALBEDO = mix(col_sample.rgb, burn.rgb, burn_mask);
    // 关键：ALPHA 用 burn_band，燃烧带保持不透明
    ALPHA  = col_sample.a * burn_band;
    // 内置 Rim
    RIM      = rim_amount * ink_sample.r * keep;
    RIM_TINT = rim_tint_value;
    // 燃烧带发光
    EMISSION = burn.rgb * burn_mask * burn_intensity;
    // ===== 金色整体提亮（径向衰减）=====
    float dist = length(local_pos - glow_center) / brightness_radius;
    float radial_falloff = 1.0 - clamp(dist, 0.0, 1.0);
    float glow_strength = max((brightness - 1.0) * radial_falloff, 0.0);
    EMISSION += glow_color.rgb * glow_strength;
}
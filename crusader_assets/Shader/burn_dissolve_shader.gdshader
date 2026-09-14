shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

// 两张贴图：col 用于 albedo，ink 用于 rim 强度遮罩
uniform sampler2D col_texture : source_color, filter_linear_mipmap, repeat_enable;
uniform sampler2D ink_texture : source_color, filter_linear_mipmap, repeat_enable;
// 溶解用噪声图（灰度即可）
uniform sampler2D noise_texture : hint_default_black, filter_linear_mipmap, repeat_enable;
// Rim 相关参数，默认与 StandardMaterial3D 一致
uniform float rim_amount : hint_range(0.0, 1.0) = 1.0;
uniform float rim_tint_value : hint_range(0.0, 1.0) = 0.0;
// 溶解 / 燃烧
uniform float dissolve_value : hint_range(0.0, 1.0) = 1.0;   // 1 = 完整，0 = 全灰烬
uniform float burn_size      : hint_range(0.0, 1.0) = 0.3;  // 燃烧边缘宽度
uniform vec4  burn_color     : source_color = vec4(1.0, 0.4, 0.05, 1.0);
uniform float burn_intensity : hint_range(0.0, 20.0) = 15.0;
// 自发光
uniform vec4 bright_color : source_color = vec4(1.0, 0.84, 0.0, 1.0);
uniform float brightness : hint_range(1.0, 20.0) = 1.0;
uniform float brightness_radius : hint_range(0.1, 2.0) = 1.0;
// 发光中心在模型局部空间的偏移量（Y 向上：模型越高，这个值越大）
uniform vec3 offset_center = vec3(0.0, 2.0, 0.0);

varying vec3 local_pos;
void vertex() {
    local_pos = VERTEX;   // 模型局部空间坐标
}

void fragment() {
    // 采样贴图（source_color 已确保 sRGB → 线性转换）
    vec4  col_sample = texture(col_texture, UV);
    vec4  ink_sample = texture(ink_texture, UV);
    float noise      = texture(noise_texture, UV).r;
    // ===== 溶解 =====
    // is_active 在 dissolve_value = 0 或 1 时为 0；
    // burn_width 用 max(..., 0.0001) 兜底，避免 smoothstep(edge0, edge1) 的 edge0 == edge1 未定义行为
    float is_active  = step(0.001, dissolve_value) * step(dissolve_value, 0.999);
    float burn_width = max(burn_size * is_active, 0.0001);
    // dissolve_value = 1 → threshold = 1（完整保留）
    // dissolve_value = 0 → threshold = 0（完全消失）
    float threshold = smoothstep(noise - burn_width, noise, dissolve_value);
    // ===== 基础色（与正确版一致，不做任何混合）=====
    ALBEDO = col_sample.rgb;
    ALPHA  = col_sample.a * threshold;   // 溶解通过 ALPHA 生效
    // ===== 内置 Rim（保留正确版的写法）=====
    // ink 红色通道作为 rim 强度遮罩；乘 threshold 让正在消失的区域同步淡出
    RIM      = rim_amount * ink_sample.r * threshold;
    RIM_TINT = rim_tint_value;
    // ===== 燃烧边缘发光（只在溶解边界处出现）=====
    // edge 只在 dissolve_value 处于中间过渡时才有值；dissolve_value = 1 时 is_active = 0 → edge = 0
    float edge = (1.0 - smoothstep(noise, noise + burn_width * 0.5, dissolve_value)) * is_active;
    EMISSION = burn_color.rgb * edge * burn_intensity;
	//----------------------
	//增加亮度和柔光效果
	// 距 UV 中心归一化距离：0 = 中心，1 = 距中心 radius 边界，>1 = 外面
	// 用模型原点减去一个自定义偏移量来调整发光点
    float dist = length(local_pos-offset_center) / brightness_radius;   // 归一化
    float radial_falloff = 1.0 - clamp(dist, 0.0, 1.0);
	float bright_factor = mix(1.0, brightness, radial_falloff);
	float glow_strength = (brightness - 1.0) * radial_falloff;
	//只在中心叠加"发光色"，原始 ALBEDO 保持不变
	ALBEDO *= bright_factor;
	EMISSION += bright_color.rgb * max(glow_strength, 0.0);
}
-- V01: 创建基础聚合视图
-- 包含：v_shipment_delivery（寄样交付）、v_creator_tag_summary（达人标签汇总）

-- ========================================
-- v_creator_tag_summary: 达人标签汇总视图
-- ========================================
DROP VIEW IF EXISTS v_creator_tag_summary;
CREATE VIEW v_creator_tag_summary AS
SELECT
    c.id AS creator_id,
    c.handle,
    c.nickname,
    c.quality_level,
    GROUP_CONCAT(DISTINCT td.name) AS tags,
    COUNT(DISTINCT ct.tag_id) AS tag_count
FROM creators c
LEFT JOIN creator_tags ct ON c.id = ct.creator_id
LEFT JOIN tag_definitions td ON ct.tag_id = td.id AND td.deleted = 0
WHERE c.deleted = 0
GROUP BY c.id, c.handle, c.nickname, c.quality_level;

-- ========================================
-- v_shipment_delivery: 寄样交付视图
-- ========================================
DROP VIEW IF EXISTS v_shipment_delivery;
CREATE VIEW v_shipment_delivery AS
SELECT
    s.id AS shipment_id,
    s.tracking_number,
    s.creator_id,
    s.sku,
    s.sku_code,
    s.product_code,
    s.sample_date,
    s.required_video_count,
    s.shipment_status,
    s.is_completed_manual,
    s.completed_reason,
    s.completed_at,
    s.notes,
    s.deleted,
    c.handle AS creator_handle,
    c.nickname AS creator_nickname,
    c.quality_level,
    -- 统计有效视频数
    COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.is_effective = 1 THEN 1 ELSE 0 END), 0) AS valid_video_count,
    -- 统计缺视频数
    COALESCE(s.required_video_count, 3) - COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.is_effective = 1 THEN 1 ELSE 0 END), 0) AS missing_video_count,
    -- 统计有视频无代码数
    COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.video_url IS NOT NULL AND v.video_url != '' AND (v.ad_code IS NULL OR v.ad_code = '') THEN 1 ELSE 0 END), 0) AS video_no_code_count,
    -- 统计已给代码数
    COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.ad_code IS NOT NULL AND v.ad_code != '' THEN 1 ELSE 0 END), 0) AS code_count,
    -- 统计总视频数
    COALESCE(COUNT(CASE WHEN v.deleted = 0 AND v.video_url IS NOT NULL AND v.video_url != '' THEN 1 END), 0) AS total_video_count,
    -- 计算寄样状态
    CASE
        WHEN s.deleted = 1 THEN '已删除'
        WHEN s.is_completed_manual = 1 THEN COALESCE(s.completed_reason, '手动完成')
        WHEN s.tracking_number IS NULL OR s.tracking_number = '' THEN '待寄样'
        WHEN COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.is_effective = 1 THEN 1 ELSE 0 END), 0) = 0 THEN '已寄样等视频'
        WHEN COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.is_effective = 1 THEN 1 ELSE 0 END), 0) < COALESCE(s.required_video_count, 3) THEN '部分交付'
        WHEN COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.is_effective = 1 THEN 1 ELSE 0 END), 0) >= COALESCE(s.required_video_count, 3) THEN '已完成'
        ELSE '待寄样'
    END AS calculated_status,
    -- 是否缺代码
    CASE
        WHEN COALESCE(SUM(CASE WHEN v.deleted = 0 AND v.video_url IS NOT NULL AND v.video_url != '' AND (v.ad_code IS NULL OR v.ad_code = '') THEN 1 ELSE 0 END), 0) > 0 THEN 1
        ELSE 0
    END AS has_missing_code,
    -- 综合 GMV
    COALESCE(SUM(CASE WHEN v.deleted = 0 THEN v.gmv ELSE 0 END), 0) AS total_gmv
FROM shipments s
LEFT JOIN creators c ON s.creator_id = c.id
LEFT JOIN videos v ON s.id = v.shipment_id
WHERE s.deleted = 0
GROUP BY s.id, s.tracking_number, s.creator_id, s.sku, s.sku_code, s.product_code,
         s.sample_date, s.required_video_count, s.shipment_status, s.is_completed_manual,
         s.completed_reason, s.completed_at, s.notes, s.deleted,
         c.handle, c.nickname, c.quality_level;

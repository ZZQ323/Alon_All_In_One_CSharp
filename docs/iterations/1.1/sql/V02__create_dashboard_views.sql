-- V02: 创建仪表盘视图（依赖 V01）
-- 包含：v_creator_dashboard、v_creator_fulfillment、v_today_follow_up

-- ========================================
-- v_creator_dashboard: 达人工作台视图
-- ========================================
DROP VIEW IF EXISTS v_creator_dashboard;
CREATE VIEW v_creator_dashboard AS
SELECT
    c.id AS creator_id,
    c.handle,
    c.nickname,
    c.quality_level,
    COALESCE(ts.tags, '') AS tags,
    c.whatsapp,
    c.email,
    c.other_contact,
    c.first_sample_date,
    c.latest_contact_date,
    c.creator_notes,
    c.tiktok_homepage_url,
    c.country_region,
    c.content_category,
    c.preferred_language,
    c.source_channel,
    c.commission_rate,
    c.preferred_name,
    c.deleted,
    -- 统计缺视频总数
    COALESCE(SUM(CASE WHEN s.shipment_status != '已完成' AND s.is_completed_manual = 0
                      THEN (COALESCE(s.required_video_count, 3) - COALESCE(sd.valid_video_count, 0))
                      ELSE 0 END), 0) AS missing_video_count,
    -- 统计缺代码总数
    COALESCE(SUM(CASE WHEN s.shipment_status != '已完成' AND s.is_completed_manual = 0
                      THEN COALESCE(sd.video_no_code_count, 0)
                      ELSE 0 END), 0) AS missing_code_count,
    -- 统计持续跟进数
    COALESCE(SUM(CASE WHEN s.shipment_status = '已完成' OR s.is_completed_manual = 1 THEN 1 ELSE 0 END), 0) AS continuous_count,
    -- 综合 GMV
    COALESCE(SUM(COALESCE(sd.total_gmv, 0)), 0) AS total_gmv,
    -- 今日是否待跟进
    CASE
        WHEN c.latest_contact_date < DATE('now') THEN 1
        ELSE 0
    END AS today_follow_up,
    -- 寄样总数
    COUNT(DISTINCT s.id) AS shipment_count,
    -- 视频总数
    COALESCE(SUM(COALESCE(sd.total_video_count, 0)), 0) AS video_count,
    -- 有效视频总数
    COALESCE(SUM(COALESCE(sd.valid_video_count, 0)), 0) AS valid_video_count
FROM creators c
LEFT JOIN shipments s ON c.id = s.creator_id AND s.deleted = 0
LEFT JOIN v_shipment_delivery sd ON s.id = sd.shipment_id
LEFT JOIN v_creator_tag_summary ts ON c.id = ts.creator_id
WHERE c.deleted = 0
GROUP BY c.id, c.handle, c.nickname, c.quality_level, ts.tags, c.whatsapp, c.email, c.other_contact,
         c.first_sample_date, c.latest_contact_date, c.creator_notes, c.tiktok_homepage_url,
         c.country_region, c.content_category, c.preferred_language, c.source_channel,
         c.commission_rate, c.preferred_name, c.deleted;

-- ========================================
-- v_creator_fulfillment: 达人履约详情视图
-- ========================================
DROP VIEW IF EXISTS v_creator_fulfillment;
CREATE VIEW v_creator_fulfillment AS
SELECT
    c.id AS creator_id,
    c.handle,
    c.nickname,
    c.quality_level,
    COALESCE(ts.tags, '') AS tags,
    s.id AS shipment_id,
    s.tracking_number,
    s.sku,
    s.sku_code,
    s.sample_date,
    s.shipment_status,
    s.required_video_count,
    sd.valid_video_count,
    sd.missing_video_count,
    sd.video_no_code_count,
    sd.code_count,
    sd.total_video_count,
    sd.calculated_status,
    sd.has_missing_code,
    sd.total_gmv,
    c.preferred_name,
    c.deleted
FROM creators c
LEFT JOIN shipments s ON c.id = s.creator_id AND s.deleted = 0
LEFT JOIN v_shipment_delivery sd ON s.id = sd.shipment_id
LEFT JOIN v_creator_tag_summary ts ON c.id = ts.creator_id
WHERE c.deleted = 0;

-- ========================================
-- v_today_follow_up: 今日待跟进视图
-- ========================================
DROP VIEW IF EXISTS v_today_follow_up;
CREATE VIEW v_today_follow_up AS
SELECT
    c.id AS creator_id,
    c.handle,
    c.nickname,
    c.preferred_name,
    c.quality_level,
    COALESCE(ts.tags, '') AS tags,
    c.latest_contact_date,
    c.whatsapp,
    c.email,
    c.country_region,
    c.content_category,
    -- 计算跟进状态
    CASE
        WHEN c.latest_contact_date IS NULL THEN 'pending'
        WHEN c.latest_contact_date < DATE('now') THEN 'pending'
        WHEN c.latest_contact_date = DATE('now') THEN 'acknowledged'
        ELSE 'completed'
    END AS follow_up_status,
    -- 计算逾期天数
    CASE
        WHEN c.latest_contact_date IS NULL THEN CAST(JULIANDAY('now') - JULIANDAY(c.first_sample_date) AS INTEGER)
        WHEN c.latest_contact_date < DATE('now') THEN CAST(JULIANDAY('now') - JULIANDAY(c.latest_contact_date) AS INTEGER)
        ELSE 0
    END AS overdue_days,
    -- 统计活跃寄样数
    COUNT(DISTINCT s.id) AS active_shipment_count,
    -- 统计待交付视频数
    COALESCE(SUM(CASE WHEN s.shipment_status != '已完成' AND s.is_completed_manual = 0
                      THEN (COALESCE(s.required_video_count, 3) - COALESCE(sd.valid_video_count, 0))
                      ELSE 0 END), 0) AS pending_video_count,
    c.deleted
FROM creators c
LEFT JOIN shipments s ON c.id = s.creator_id AND s.deleted = 0
LEFT JOIN v_shipment_delivery sd ON s.id = sd.shipment_id
LEFT JOIN v_creator_tag_summary ts ON c.id = ts.creator_id
WHERE c.deleted = 0
  AND (c.latest_contact_date IS NULL OR c.latest_contact_date <= DATE('now'))
GROUP BY c.id, c.handle, c.nickname, c.preferred_name, c.quality_level, ts.tags,
         c.latest_contact_date, c.whatsapp, c.email, c.country_region, c.content_category, c.deleted;

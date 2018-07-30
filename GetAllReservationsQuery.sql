DECLARE @park_id int = 1
DECLARE @currentDate DATE = '6/20/2018'
DECLARE @thirtyDate DATE = '7/20/2018'

SELECT reservation_id, reservation.site_id, reservation.name, from_date, to_date, create_date, campground.name AS campgroundName 
FROM reservation 
JOIN site ON reservation.site_id = site.site_id 
JOIN campground ON site.campground_id = campground.campground_id
JOIN park ON park.park_id = campground.park_id 
WHERE park.park_id = @park_id
AND @currentDate <= reservation.from_date 
AND @thirtyDate >= reservation.from_date 
ORDER BY from_date DESC;
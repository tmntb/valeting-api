use Valeting

select b.ID, b.Name, b.BookingDate, b.ContactNumber, b.Email, b.Approved, f.[Description], vh.[Description]
from Booking B
inner join RD_Flexibility f on b.Flexibility_ID = f.ID
inner JOIN RD_VehicleSize vH on b.VehicleSize_ID = vH.ID
order by b.BookingDate

select * from RD_VehicleSize
SELECT * from RD_Flexibility
use Valeting

select b.ID, b.Name, b.BookingDate, b.ContactNumber, b.Email, b.Approved, f.[Description], vh.[Description]
from Booking B
inner join RD_Flexibility f on b.Flexibility_ID = f.ID
inner JOIN RD_VehicleSize vH on b.VehicleSize_ID = vH.ID
where b.ID = '93698543-d39a-4151-870a-200f8cb9cebf'
order by b.BookingDate

--select * from RD_VehicleSize
--SELECT * from RD_Flexibility
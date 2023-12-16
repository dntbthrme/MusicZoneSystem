SELECT p.prod_name, p.prod_desc, c.canvas_quantity, c.canvas_unit, p.prod_price, c.canvas_total FROM canvas c JOIN product p ON c.prod_id = p.prod_id

DELETE FROM [dbo].canvas;

-- Reset the identity seed value to 1
DBCC CHECKIDENT('[dbo].[canvas]', RESEED, 0);
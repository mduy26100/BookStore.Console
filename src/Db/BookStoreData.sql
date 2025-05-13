USE BookStore

GO

-- Thêm dữ liệu mẫu vào Tài Khoản
INSERT INTO Accounts (Username, Password, Name, Email, Address, PhoneNumber, Role)
VALUES ('admin', 'admin', 'ADMIN', 'admin@example.com', 'Admin Address', 'Admin Phone', 'admin');

GO

INSERT INTO Accounts (Username, Password, Name, Email, Address, PhoneNumber, Role)
VALUES ('customer', 'customer', 'CUSTOMER', 'customer@example.com', 'Customer Address', 'Customer Phone', 'customer');

GO

INSERT INTO Accounts (Username, Password, Name, Email, Address, PhoneNumber, Role)
VALUES ('customer1', 'customer1', 'CUSTOMERS', 'customer1@example.com', 'Customer Address 1', 'Customer Phone1', 'customer');

GO

-- Thêm sách vào bảng Books
INSERT INTO Books (Title, Author, Price, Stock, Description)
VALUES
    ('Sapiens: A Brief History of Humankind', 'Yuval Noah Harari', 15.99, 50, 'A groundbreaking narrative of humanity''s history from the emergence of Homo sapiens to the present day.'),
    ('The Diary of a Young Girl', 'Anne Frank', 9.99, 30, 'Anne Frank''s extraordinary diary, written while hiding from the Nazis during World War II, offers a poignant insight into the life of a young girl facing adversity.'),
    ('The Guns of August', 'Barbara W. Tuchman', 12.50, 40, 'A detailed account of the events leading up to World War I, offering a gripping narrative of the political and military decisions that shaped the course of history.'),
    ('A Brief History of Time', 'Stephen Hawking', 11.99, 35, 'Stephen Hawking explores the universe''s mysteries, from the Big Bang to black holes, in this accessible and enlightening book on theoretical physics.'),
    ('The Gene: An Intimate History', 'Siddhartha Mukherjee', 14.50, 25, 'Delving into the history and science of genetics, Siddhartha Mukherjee provides a comprehensive account of the gene''s role in shaping life on Earth.'),
    ('Cosmos', 'Carl Sagan', 13.75, 45, 'A captivating journey through space and time, Carl Sagan''s "Cosmos" explores the wonders of the universe and our place within it.'),
    ('The Power of Habit', 'Charles Duhigg', 10.99, 50, 'Charles Duhigg investigates the science behind habit formation and how understanding habits can transform our lives.'),
    ('Quiet: The Power of Introverts in a World That Can''t Stop Talking', 'Susan Cain', 9.75, 30, 'Susan Cain explores the strengths and talents of introverts in a society that often values extroversion, offering insights into how introverts can thrive.'),
    ('Ikigai: The Japanese Secret to a Long and Happy Life', 'Héctor García and Francesc Miralles', 12.25, 40, 'Drawing on Japanese philosophy, "Ikigai" explores the concept of finding purpose and joy in everyday life.'),
	('To Kill a Mockingbird', 'Harper Lee', 12.99, 50, 'Harper Lee''s classic novel explores themes of racial injustice, moral growth, and compassion in the fictional town of Maycomb, Alabama, during the 1930s.'),
    ('The Great Gatsby', 'F. Scott Fitzgerald', 11.50, 45, 'F. Scott Fitzgerald''s iconic novel captures the glamour and excess of the Jazz Age, while also delving into themes of love, wealth, and the American Dream.'),
    ('Pride and Prejudice', 'Jane Austen', 10.75, 55, 'Jane Austen''s beloved novel follows the romantic entanglements of the Bennet sisters in Georgian England, exploring themes of class, marriage, and personal integrity.'),
	('Good to Great: Why Some Companies Make the Leap... and Others Don''t', 'Jim Collins', 14.99, 60, 'Jim Collins analyzes the factors that distinguish great companies from merely good ones, offering insights into effective leadership, strategy, and organizational culture.'),
    ('Lean In: Women, Work, and the Will to Lead', 'Sheryl Sandberg', 13.25, 40, 'Sheryl Sandberg examines the barriers that hold women back in the workplace and offers practical advice for achieving gender equality and leadership success.'),
    ('The 7 Habits of Highly Effective People: Powerful Lessons in Personal Change', 'Stephen R. Covey', 15.50, 65, 'Stephen R. Covey presents a holistic approach to personal and professional effectiveness, outlining seven habits that can transform individuals and organizations.');


GO

--Thêm vào chủ đề vào bảng Categories
--Lịch sử
INSERT INTO Categories(Name, Description)
VALUES ('History', 'Books about history')

GO

--Khoa học
INSERT INTO Categories(Name, Description)
VALUES ('Science', 'Books about science')

GO

--Đời sống
INSERT INTO Categories(Name, Description)
VALUES ('Life', 'Books about life')

GO

--Tiểu thuyết
INSERT INTO Categories(Name, Description)
VALUES ('Novel', 'Books about novel')

GO

--Kinh doanh và Lãnh đạo
INSERT INTO Categories(Name, Description)
VALUES ('Business and Leadership', 'Books about Business and Leadership')

GO

-- Thêm sách vào bảng BookCategories với các chủ đề tương ứng
-- Lịch sử
INSERT INTO BookCategories (BookID, CategoryID)
VALUES
    (1, 1), -- Sapiens: A Brief History of Humankind
    (2, 1), -- The Diary of a Young Girl
    (3, 1); -- The Guns of August

GO

-- Khoa học
INSERT INTO BookCategories (BookID, CategoryID)
VALUES
    (4, 2), -- A Brief History of Time
    (5, 2), -- The Gene: An Intimate History
    (6, 2); -- Cosmos

GO

-- Đời sống
INSERT INTO BookCategories (BookID, CategoryID)
VALUES
    (7, 3), -- The Power of Habit
    (8, 3), -- Quiet: The Power of Introverts in a World That Can't Stop Talking
    (9, 3); -- Ikigai: The Japanese Secret to a Long and Happy Life

GO

-- Tiểu thuyết
INSERT INTO BookCategories (BookID, CategoryID)
VALUES
    (10, 4), -- To Kill a Mockingbird
    (11, 4), -- The Great Gatsby
    (12, 4); -- Pride and Prejudice

GO

-- Kinh doanh và Lãnh đạo
INSERT INTO BookCategories (BookID, CategoryID)
VALUES
    (13, 5), -- Good to Great
    (14, 5), -- Lean In
    (15, 5); -- The 7 Habits of Highly Effective People

GO

-- Thêm sách vào giỏ hàng cho tài khoản customer và customer1
INSERT INTO ShoppingCarts (AccountID, BookID, Quantity)
VALUES 
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 1, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 2, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 3, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 4, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 5, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 6, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 7, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 8, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 9, 1),
    ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 10, 1);

GO

-- Thêm đơn hàng cho tài khoản customer với trạng thái Pending
-- Đơn hàng 1
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 15.99, 'Pending');

DECLARE @OrderID1 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID1, 1, 1, 15.99);

-- Đơn hàng 2
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 9.99, 'Pending');

DECLARE @OrderID2 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID2, 2, 1, 9.99);

-- Đơn hàng 3
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 12.50, 'Pending');

DECLARE @OrderID3 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID3, 3, 1, 12.50);

-- Đơn hàng 4
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 11.99, 'Pending');

DECLARE @OrderID4 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID4, 4, 1, 11.99);

-- Đơn hàng 5
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 14.50, 'Pending');

DECLARE @OrderID5 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID5, 5, 1, 14.50);

GO

-- Thêm đơn hàng cho tài khoản customer với trạng thái Completed
-- Đơn hàng 6
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 13.75, 'Completed');

DECLARE @OrderID6 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID6, 6, 1, 13.75);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID6, 6, 1, 13.75, GETDATE(), 'Excellent book on space and time.');

-- Đơn hàng 7
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 10.99, 'Completed');

DECLARE @OrderID7 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID7, 7, 1, 10.99);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID7, 7, 1, 10.99, GETDATE(), 'Very insightful and practical.');

-- Đơn hàng 8
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 9.75, 'Completed');

DECLARE @OrderID8 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID8, 8, 1, 9.75);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID8, 8, 1, 9.75, GETDATE(), 'Great book for introverts.');

-- Đơn hàng 9
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 12.25, 'Completed');

DECLARE @OrderID9 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID9, 9, 1, 12.25);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID9, 9, 1, 12.25, GETDATE(), 'Inspiring and motivational.');

-- Đơn hàng 10
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 12.99, 'Completed');

DECLARE @OrderID10 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID10, 10, 1, 12.99);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID10, 10, 1, 12.99, GETDATE(), 'A classic that everyone should read.');

GO

-- Thêm đơn hàng cho tài khoản customer với trạng thái Confirmed
-- Đơn hàng 11
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 15.99, 'Confirmed');

DECLARE @OrderID11 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID11, 1, 1, 15.99);

-- Đơn hàng 12
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 9.99, 'Confirmed');

DECLARE @OrderID12 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID12, 2, 1, 9.99);

-- Đơn hàng 13
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 12.50, 'Confirmed');

DECLARE @OrderID13 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID13, 3, 1, 12.50);

-- Đơn hàng 14
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 11.99, 'Confirmed');

DECLARE @OrderID14 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID14, 4, 1, 11.99);

-- Đơn hàng 15
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer'), 'Customer Phone', 'customer@example.com', 'Customer Address', 'Customer Name', GETDATE(), 14.50, 'Confirmed');

DECLARE @OrderID15 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID15, 5, 1, 14.50);

GO

-- Thêm đơn hàng cho tài khoản customer1 với trạng thái Pending
-- Đơn hàng 16
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 11.50, 'Pending');

DECLARE @OrderID16 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID16, 11, 1, 11.50);

-- Đơn hàng 17
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 10.75, 'Pending');

DECLARE @OrderID17 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID17, 12, 1, 10.75);

-- Đơn hàng 18
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 14.99, 'Pending');

DECLARE @OrderID18 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID18, 13, 1, 14.99);

-- Đơn hàng 19
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 13.25, 'Pending');

DECLARE @OrderID19 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID19, 14, 1, 13.25);

-- Đơn hàng 20
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 15.50, 'Pending');

DECLARE @OrderID20 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID20, 15, 1, 15.50);

GO

-- Thêm đơn hàng cho tài khoản customer1 với trạng thái Completed
-- Đơn hàng 21
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 15.99, 'Completed');

DECLARE @OrderID21 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID21, 1, 1, 15.99);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID21, 1, 1, 15.99, GETDATE(), 'A fascinating journey through human history.');

-- Đơn hàng 22
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 9.99, 'Completed');

DECLARE @OrderID22 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID22, 2, 1, 9.99);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID22, 2, 1, 9.99, GETDATE(), 'A poignant and heart-wrenching diary.');

-- Đơn hàng 23
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 12.50, 'Completed');

DECLARE @OrderID23 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID23, 3, 1, 12.50);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID23, 3, 1, 12.50, GETDATE(), 'A comprehensive account of WWI.');

-- Đơn hàng 24
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 11.99, 'Completed');

DECLARE @OrderID24 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID24, 4, 1, 11.99);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID24, 4, 1, 11.99, GETDATE(), 'A must-read for anyone interested in physics.');

-- Đơn hàng 25
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 14.50, 'Completed');

DECLARE @OrderID25 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID25, 5, 1, 14.50);

INSERT INTO Reports (OrderID, BookID, Quantity, Price, OrderDate, CustomerReviews)
VALUES (@OrderID25, 5, 1, 14.50, GETDATE(), 'An in-depth look into the science of genetics.');

GO

-- Thêm đơn hàng cho tài khoản customer1 với trạng thái Confirmed
-- Đơn hàng 26
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 13.75, 'Confirmed');

DECLARE @OrderID26 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID26, 6, 1, 13.75);

-- Đơn hàng 27
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 10.99, 'Confirmed');

DECLARE @OrderID27 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID27, 7, 1, 10.99);

-- Đơn hàng 28
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 9.75, 'Confirmed');

DECLARE @OrderID28 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID28, 8, 1, 9.75);

-- Đơn hàng 29
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 12.25, 'Confirmed');

DECLARE @OrderID29 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID29, 9, 1, 12.25);

-- Đơn hàng 30
INSERT INTO Orders (AccountID, CustomerPhone, CustomerEmail, CustomerAddress, CustomerName, OrderDate, TotalPrice, Status)
VALUES ((SELECT AccountID FROM Accounts WHERE Username = 'customer1'), 'Customer Phone1', 'customer1@example.com', 'Customer Address 1', 'Customer Name1', GETDATE(), 12.99, 'Confirmed');

DECLARE @OrderID30 INT = (SELECT SCOPE_IDENTITY());

INSERT INTO OrderDetails (OrderID, BookID, Quantity, Price)
VALUES (@OrderID30, 10, 1, 12.99);

GO

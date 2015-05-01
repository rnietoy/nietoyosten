------------------------------- Schema Change script ------------------------------------
--
-- Title:          Add unique constraint for picture filename within an album
-- Description:    Add unique constraint for picture filename within an album
-- Schema version: 01.00.007

ALTER TABLE [Pictures] ADD CONSTRAINT [UQ_Pictures_AlbumID_FileName] UNIQUE NONCLUSTERED ([AlbumID], [FileName] ASC)

if not exists(select syscolumns.id from syscolumns join sysobjects on  syscolumns.ID = sysobjects.ID  where  sysobjects.name = 'TPL_TRUCKPLAN' and  syscolumns.name = 'TPL_PCOLOR') begin
	ALTER TABLE TPL_TRUCKPLAN ADD TPL_PCOLOR int default -1;
end
go


if not exists(select syscolumns.id from syscolumns join sysobjects on  syscolumns.ID = sysobjects.ID  where  sysobjects.name = 'TPL_TRUCKPLAN' and  syscolumns.name = 'TPL_PSELECT') begin
	ALTER TABLE TPL_TRUCKPLAN ADD TPL_PSELECT TY_BVALUE default -1;
end
go


if not exists(select syscolumns.id from syscolumns join sysobjects on  syscolumns.ID = sysobjects.ID  where  sysobjects.name = 'TRK_TRUCK' and  syscolumns.name = 'TRK_COLOR') begin
	ALTER TABLE TRK_TRUCK ADD TRK_COLOR int default -1;
end
go

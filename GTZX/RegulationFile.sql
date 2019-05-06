CREATE TABLE [dbo].Biz_RegulationFile (
    [Id]          UNIQUEIDENTIFIER PRIMARY KEY	NOT NULL,
    [ParentId]    UNIQUEIDENTIFIER NULL,
    [RegulationFileName]        NVARCHAR (50)    NOT NULL,
    RegulationFileNo FLOAT (53)       NULL,
    [IsDelete]    BIT              NOT NULL,
	[RegulationSourceFile] NVARCHAR(500) NOT NULL,
    [Remark]      NVARCHAR (500)   NULL
)

SELECT * FROM dbo.Biz_RegulationFile

SELECT * FROM dbo.Sys_Func
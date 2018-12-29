create database flow
go
use flow
go
if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_actor')
            and   type = 'U')
   drop table dbo.t_actor
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_command')
            and   type = 'U')
   drop table dbo.t_command
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_config')
            and   type = 'U')
   drop table dbo.t_config
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_group')
            and   type = 'U')
   drop table dbo.t_group
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_instance')
            and   type = 'U')
   drop table dbo.t_instance
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_node')
            and   type = 'U')
   drop table dbo.t_node
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_process')
            and   type = 'U')
   drop table dbo.t_process
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_structure')
            and   type = 'U')
   drop table dbo.t_structure
go

if exists (select 1
            from  sysobjects
           where  id = object_id('dbo.t_transition')
            and   type = 'U')
   drop table dbo.t_transition
go
/*==============================================================*/
/* Table: t_actor                                               */
/*==============================================================*/
create table dbo.t_actor (
   NID                  varchar(50)          collate Chinese_PRC_CI_AS not null,
   RNID                 varchar(50)          collate Chinese_PRC_CI_AS null,
   IDENTIFICATION       varchar(50)          null,
   APPELLATION          varchar(50)          collate Chinese_PRC_CI_AS null,
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS null,
   CREATEDATETIME       datetime             null constraint DF_t_actor_INSERTDATE default getdate(),
   OPERATION            varchar(50)          collate Chinese_PRC_CI_AS null,
   constraint PK_t_actor primary key (NID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '审批参与者',
   'user', 'dbo', 'table', 't_actor'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', 'dbo', 'table', 't_actor', 'column', 'NID'
go

execute sp_addextendedproperty 'MS_Description', 
   '外键',
   'user', 'dbo', 'table', 't_actor', 'column', 'RNID'
go

execute sp_addextendedproperty 'MS_Description', 
   '参与者标识',
   'user', 'dbo', 'table', 't_actor', 'column', 'IDENTIFICATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '参与者的名称',
   'user', 'dbo', 'table', 't_actor', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例ID 与 T_INSTANCE表关联',
   'user', 'dbo', 'table', 't_actor', 'column', 'INSTANCEID'
go

execute sp_addextendedproperty 'MS_Description', 
   '参与者审批时间',
   'user', 'dbo', 'table', 't_actor', 'column', 'CREATEDATETIME'
go

execute sp_addextendedproperty 'MS_Description', 
   '参与者动作（撤销、退回、跳转）',
   'user', 'dbo', 'table', 't_actor', 'column', 'OPERATION'
go

/*==============================================================*/
/* Table: t_command                                             */
/*==============================================================*/
create table dbo.t_command (
   NID                  varchar(50)          collate Chinese_PRC_CI_AS not null,
   RNID                 varchar(50)          collate Chinese_PRC_CI_AS null,
   APPELLATION          varchar(50)          collate Chinese_PRC_CI_AS null,
   SCRIPT               varchar(4000)        collate Chinese_PRC_CI_AS null,
   CONNECTE             varchar(512)         collate Chinese_PRC_CI_AS null,
   PROVIDERNAME         varchar(50)          collate Chinese_PRC_CI_AS null,
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS null,
   COMMANDTYPE          varchar(50)          collate Chinese_PRC_CI_AS null,
   constraint PK_t_command primary key (NID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '分支多条件命令',
   'user', 'dbo', 'table', 't_command'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', 'dbo', 'table', 't_command', 'column', 'NID'
go

execute sp_addextendedproperty 'MS_Description', 
   '外键，与t_node 表进行关键，即决策节点，对应的命令集合',
   'user', 'dbo', 'table', 't_command', 'column', 'RNID'
go

execute sp_addextendedproperty 'MS_Description', 
   '命令名称',
   'user', 'dbo', 'table', 't_command', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   'SQL文本',
   'user', 'dbo', 'table', 't_command', 'column', 'SCRIPT'
go

execute sp_addextendedproperty 'MS_Description', 
   '连接字符串',
   'user', 'dbo', 'table', 't_command', 'column', 'CONNECTE'
go

execute sp_addextendedproperty 'MS_Description', 
   '访问客户端如(System.Data.SqlClient)',
   'user', 'dbo', 'table', 't_command', 'column', 'PROVIDERNAME'
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例ID 与 T_INSTANCE表关联',
   'user', 'dbo', 'table', 't_command', 'column', 'INSTANCEID'
go

execute sp_addextendedproperty 'MS_Description', 
   '命令类型，目前只支持（text）文本SQL',
   'user', 'dbo', 'table', 't_command', 'column', 'COMMANDTYPE'
go

/*==============================================================*/
/* Table: t_config                                              */
/*==============================================================*/
create table dbo.t_config (
   IDENTIFICATION       bigint               not null,
   APPELLATION          varchar(50)          collate Chinese_PRC_CI_AS null,
   CONNECTE             varchar(512)         collate Chinese_PRC_CI_AS null,
   PROVIDERNAME         varchar(50)          collate Chinese_PRC_CI_AS null,
   constraint PK_t_config primary key (IDENTIFICATION)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流数据库配置表',
   'user', 'dbo', 'table', 't_config'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', 'dbo', 'table', 't_config', 'column', 'IDENTIFICATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '数据源名称',
   'user', 'dbo', 'table', 't_config', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '连接字符串',
   'user', 'dbo', 'table', 't_config', 'column', 'CONNECTE'
go

execute sp_addextendedproperty 'MS_Description', 
   '访问客户端如(System.Data.SqlClient)',
   'user', 'dbo', 'table', 't_config', 'column', 'PROVIDERNAME'
go

/*==============================================================*/
/* Table: t_group                                               */
/*==============================================================*/
create table dbo.t_group (
   NID                  varchar(50)          collate Chinese_PRC_CI_AS not null,
   RNID                 varchar(50)          collate Chinese_PRC_CI_AS null,
   IDENTIFICATION       varchar(50)          null,
   APPELLATION          varchar(50)          collate Chinese_PRC_CI_AS null,
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS null,
   constraint PK_t_role primary key (NID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '参与组',
   'user', 'dbo', 'table', 't_group'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', 'dbo', 'table', 't_group', 'column', 'NID'
go

execute sp_addextendedproperty 'MS_Description', 
   '外键，与t_node 表进行关键，即一个节点多个参与组',
   'user', 'dbo', 'table', 't_group', 'column', 'RNID'
go

execute sp_addextendedproperty 'MS_Description', 
   '组的标识',
   'user', 'dbo', 'table', 't_group', 'column', 'IDENTIFICATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '组的名称',
   'user', 'dbo', 'table', 't_group', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例ID 与 T_INSTANCE表关联',
   'user', 'dbo', 'table', 't_group', 'column', 'INSTANCEID'
go

/*==============================================================*/
/* Table: t_instance                                            */
/*==============================================================*/
create table dbo.t_instance (
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS not null,
   CREATEDATETIME       datetime             null constraint DF_t_instance_CreateDateTime default getdate(),
   RNID                 varchar(50)          null,
   STRUCTUREID          varchar(50)          collate Chinese_PRC_CI_AS null,
   STATE                varchar(50)          collate Chinese_PRC_CI_AS null constraint DF_t_instance_STATUS default 'running',
   STRUCTUREXML         text                 collate Chinese_PRC_CI_AS null,
   constraint PK_t_instance primary key (INSTANCEID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例表',
   'user', 'dbo', 'table', 't_instance'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键，实例ID',
   'user', 'dbo', 'table', 't_instance', 'column', 'INSTANCEID'
go

execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', 'dbo', 'table', 't_instance', 'column', 'CREATEDATETIME'
go

execute sp_addextendedproperty 'MS_Description', 
   '与T_NODE进行关联，即当前执行流程节点ID',
   'user', 'dbo', 'table', 't_instance', 'column', 'RNID'
go

execute sp_addextendedproperty 'MS_Description', 
   '流程模板ID t_structure.ID',
   'user', 'dbo', 'table', 't_instance', 'column', 'STRUCTUREID'
go

execute sp_addextendedproperty 'MS_Description', 
   '流程状态（运行中：running、结束：end、终止：termination,kill:杀死流程）',
   'user', 'dbo', 'table', 't_instance', 'column', 'STATE'
go

execute sp_addextendedproperty 'MS_Description', 
   '存储描述流程数据结构',
   'user', 'dbo', 'table', 't_instance', 'column', 'STRUCTUREXML'
go

/*==============================================================*/
/* Table: t_node                                                */
/*==============================================================*/
create table dbo.t_node (
   NID                  varchar(50)          collate Chinese_PRC_CI_AS not null,
   IDENTIFICATION       varchar(50)          not null,
   APPELLATION          varchar(50)          collate Chinese_PRC_CI_AS null,
   NODETYPE             varchar(50)          collate Chinese_PRC_CI_AS null,
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS null,
   constraint PK_t_node primary key (NID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '流程节点表',
   'user', 'dbo', 'table', 't_node'
go

execute sp_addextendedproperty 'MS_Description', 
   '节点的标识',
   'user', 'dbo', 'table', 't_node', 'column', 'IDENTIFICATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '节点的名称',
   'user', 'dbo', 'table', 't_node', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '节点类型（Start\End\Normal\Decision）',
   'user', 'dbo', 'table', 't_node', 'column', 'NODETYPE'
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例ID 与 T_INSTANCE表关联',
   'user', 'dbo', 'table', 't_node', 'column', 'INSTANCEID'
go

/*==============================================================*/
/* Table: t_process                                             */
/*==============================================================*/
create table dbo.t_process (
   NID                  varchar(50)          collate Chinese_PRC_CI_AS not null,
   ORIGIN               varchar(50)          null,
   DESTINATION          varchar(50)          null,
   TRANSITIONID         varchar(50)          collate Chinese_PRC_CI_AS null,
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS null,
   NODETYPE             varchar(50)          collate Chinese_PRC_CI_AS null,
   CREATEDATETIME       datetime             null constraint DF_t_process_INSERTDATE default getdate(),
   RNID                 varchar(50)          collate Chinese_PRC_CI_AS null,
   OPERATION            varchar(50)          collate Chinese_PRC_CI_AS null constraint DF_t_process_OPERATE default 'normal',
   constraint PK_t_process primary key (NID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '记录所有审批操作',
   'user', 'dbo', 'table', 't_process'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', 'dbo', 'table', 't_process', 'column', 'NID'
go

execute sp_addextendedproperty 'MS_Description', 
   '当前节点ID ',
   'user', 'dbo', 'table', 't_process', 'column', 'ORIGIN'
go

execute sp_addextendedproperty 'MS_Description', 
   '跳转节点的ID',
   'user', 'dbo', 'table', 't_process', 'column', 'DESTINATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '跳转路线的ID t_transition.NID',
   'user', 'dbo', 'table', 't_process', 'column', 'TRANSITIONID'
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例ID 与 T_INSTANCE表关联',
   'user', 'dbo', 'table', 't_process', 'column', 'INSTANCEID'
go

execute sp_addextendedproperty 'MS_Description', 
   '节点类型',
   'user', 'dbo', 'table', 't_process', 'column', 'NODETYPE'
go

execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', 'dbo', 'table', 't_process', 'column', 'CREATEDATETIME'
go

execute sp_addextendedproperty 'MS_Description', 
   '跳转到的节点NID',
   'user', 'dbo', 'table', 't_process', 'column', 'RNID'
go

execute sp_addextendedproperty 'MS_Description', 
   '动作（退回、撤销、跳转）',
   'user', 'dbo', 'table', 't_process', 'column', 'OPERATION'
go

/*==============================================================*/
/* Table: t_structure                                           */
/*==============================================================*/
create table dbo.t_structure (
   IDENTIFICATION       varchar(50)          collate Chinese_PRC_CI_AS not null,
   APPELLATION          varchar(50)          collate Chinese_PRC_CI_AS null,
   STRUCTUREXML         text                 collate Chinese_PRC_CI_AS null,
   constraint PK_T_STRUCTURE primary key (IDENTIFICATION)
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '流程模板',
   'user', 'dbo', 'table', 't_structure'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键标识 GUID',
   'user', 'dbo', 'table', 't_structure', 'column', 'IDENTIFICATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '流程图模板名称',
   'user', 'dbo', 'table', 't_structure', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '存储描述流程数据结构',
   'user', 'dbo', 'table', 't_structure', 'column', 'STRUCTUREXML'
go

/*==============================================================*/
/* Table: t_transition                                          */
/*==============================================================*/
create table dbo.t_transition (
   NID                  varchar(50)          collate Chinese_PRC_CI_AS not null,
   RNID                 varchar(50)          collate Chinese_PRC_CI_AS null,
   APPELLATION          varchar(128)         collate Chinese_PRC_CI_AS null,
   DESTINATION          varchar(50)          null,
   ORIGIN               varchar(50)          null,
   INSTANCEID           varchar(50)          collate Chinese_PRC_CI_AS null,
   EXPRESSION           varchar(50)          collate Chinese_PRC_CI_AS null,
   constraint PK_t_transition_1 primary key (NID)
         on "PRIMARY"
)
on "PRIMARY"
go

execute sp_addextendedproperty 'MS_Description', 
   '流程跳转路线',
   'user', 'dbo', 'table', 't_transition'
go

execute sp_addextendedproperty 'MS_Description', 
   '主键',
   'user', 'dbo', 'table', 't_transition', 'column', 'NID'
go

execute sp_addextendedproperty 'MS_Description', 
   '线的名称',
   'user', 'dbo', 'table', 't_transition', 'column', 'APPELLATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '跳转到节点ID',
   'user', 'dbo', 'table', 't_transition', 'column', 'DESTINATION'
go

execute sp_addextendedproperty 'MS_Description', 
   '当前节点ID',
   'user', 'dbo', 'table', 't_transition', 'column', 'ORIGIN'
go

execute sp_addextendedproperty 'MS_Description', 
   '工作流实例ID 与 T_INSTANCE表关联',
   'user', 'dbo', 'table', 't_transition', 'column', 'INSTANCEID'
go

execute sp_addextendedproperty 'MS_Description', 
   '表达式（只有分支才用）',
   'user', 'dbo', 'table', 't_transition', 'column', 'EXPRESSION'
go

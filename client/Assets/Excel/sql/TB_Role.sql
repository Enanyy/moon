DROP TABLE IF EXISTS 'TB_Role';
CREATE TABLE TB_Role (id INT NOT NULL DEFAULT (0),name VARCHAR(256) NOT NULL ,type INT NOT NULL DEFAULT (1),height DECIMAL NOT NULL DEFAULT (1),desc TEXT  ,weigth DOUBLE NOT NULL DEFAULT (0),config INT NOT NULL DEFAULT (0),UNIQUE(id));
INSERT INTO TB_Role (id,name,type,height,desc,weigth,config) VALUES (1,'TestName1',1,1.8,'fqwfqwf''asd',5.44656566565656,7);
INSERT INTO TB_Role (id,name,type,height,desc,weigth,config) VALUES (2,'TestName2',1,1.8,'中文''',5.44656566565656,5);

rs.initiate({_id: "rs-sh-01", version: 1, members: [ { _id: 0, host : "mongo-shard1-rs1:27017" }, { _id: 1, host : "mongo-shard1-rs2:27017" }] })
db.getMongo().setReadPref('primaryPreferred')
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class BoardModelTests {
    
    Reciever reciever;
    BoardModel boardModel;

    [OneTimeSetUp]
    public void setUp () {
        reciever = new Reciever();
        boardModel = new BoardModel(3, reciever);
        setFixtures();
    }

    [TearDown]
    public void setFixtures () {
        reciever.moveBlocksD = (Vector2Int from, Vector2Int to) => {};
        reciever.newGenerationD = (Vector2Int position, int offset) => {};
        boardModel.setCell(2, 2, BlockType.green);
        boardModel.setCell(1, 2, BlockType.blue);
        boardModel.setCell(0, 2, BlockType.red);
        boardModel.setCell(2, 1, BlockType.red);
        boardModel.setCell(1, 1, BlockType.red);
        boardModel.setCell(0, 1, BlockType.green);
        boardModel.setCell(2, 0, BlockType.blue);
        boardModel.setCell(1, 0, BlockType.red);
        boardModel.setCell(0, 0, BlockType.red);
        //r b g
        //g r r
        //r r b
    }

    [Test]
    public void getCell() {
        var result = boardModel.getCell(0, 0);
        Assert.AreEqual(BlockType.red, result);
    }

    [Test]
    public void setCellWithCollecting () {
        boardModel.setCellWithCollecting(2, 0, BlockType.red);
        boardModel.commitCollection();
        var result = boardModel.getCell(0, 0);
        Assert.AreEqual(BlockType.green, result);
    }

    [Test]
    public void collect_cancel () {
        var start = new Vector2Int(2, 2);
        var end = new Vector2Int(1, 2);
        var result = boardModel.collect(start, end);
        Assert.False(result);
    }

    [Test]
    public void collect_switch () {
        var start = new Vector2Int(2, 0);
        var end = new Vector2Int(2, 1);
        var result = boardModel.collect(start, end);
        Assert.True(result);
    }

    [Test]
    public void collect () {
        var start = new Vector2Int(2, 0);
        var end = new Vector2Int(2, 1);
        boardModel.collect(start, end);
        boardModel.commitCollection();
        var result = boardModel.getCell(0, 0);
        Assert.AreEqual(BlockType.green, result);
    }

    [Test]
    public void moveBlocks () {
        Vector2Int[] fromCell = new Vector2Int[3];
        Vector2Int[] toCell = new Vector2Int[3];
        int count = 0;
        reciever.moveBlocksD = (Vector2Int from, Vector2Int to) => {
            fromCell[count] = from;
            toCell[count] = to;
            count++;
        };

        var start = new Vector2Int(0, 0);
        var end = new Vector2Int(0, 1);
        boardModel.collect(start, end);
        boardModel.commitCollection();
        Assert.AreEqual(3, count);
        Assert.AreEqual(new Vector2Int(0, 2), fromCell[0]);
        Assert.AreEqual(new Vector2Int(2, 1), toCell[2]);
    }

    [Test]
    public void newGeneration () {
        Vector2Int[] cellList = new Vector2Int[3];
        int offset = 0;
        int count = 0;
        reciever.newGenerationD = (Vector2Int cell, int o) => {
            cellList[count] = cell;
            offset = o;
            count++;
        };

        var start = new Vector2Int(0, 0);
        var end = new Vector2Int(0, 1);
        boardModel.collect(start, end);
        boardModel.commitCollection();
        Assert.AreEqual(3, count);
        Assert.AreEqual(new Vector2Int(0, 2), cellList[0]);
        Assert.AreEqual(1, offset);
    }
}

delegate void MoveBlocks (Vector2Int from, Vector2Int to);
delegate void NewGeneration (Vector2Int position, int offset);

class Reciever: BoardEventReceiver {

    public MoveBlocks moveBlocksD;
    public NewGeneration newGenerationD;
    
    public void moveBlocks (Vector2Int from, Vector2Int to) {
        moveBlocksD(from, to);
    }
    public void newGeneration (Vector2Int position, int offset){
        newGenerationD(position, offset);
    }
}
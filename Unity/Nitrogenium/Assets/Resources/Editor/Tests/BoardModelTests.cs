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
        reciever.resetBlockD = (Vector2Int position, BlockType type, Vector2Int offset) => {};
        reciever.moveLenghtD = (int lenght) => {};
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
        boardModel.collect(start, end);
        var result = boardModel.commitCollection();
        Assert.AreEqual(0, result);
    }

    [Test]
    public void collect_switch () {
        var start = new Vector2Int(2, 0);
        var end = new Vector2Int(2, 1);
        boardModel.collect(start, end);
        var result = boardModel.commitCollection();
        Assert.AreEqual(3, result);
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
    public void moveLenght () {
        int count = 0;
        reciever.moveLenghtD = (int c) => {
            count = c;
        };

        var start = new Vector2Int(0, 0);
        var end = new Vector2Int(0, 1);
        boardModel.collect(start, end);
        boardModel.commitCollection();
        Assert.AreEqual(1, count);
    }

    [Test]
    public void resetBlock () {
        Vector2Int[] cellList = new Vector2Int[8];
        Vector2Int offset = Vector2Int.zero;
        int count = 0;
        reciever.resetBlockD = (Vector2Int position, BlockType type, Vector2Int o) => {
            cellList[count] = position;
            offset = o;
            count++;
        };

        var start = new Vector2Int(0, 0);
        var end = new Vector2Int(0, 1);
        boardModel.collect(start, end);
        boardModel.commitCollection();
        Assert.AreEqual(8, count);
        Assert.AreEqual(new Vector2Int(0, 0), cellList[0]);
        Assert.AreEqual(new Vector2Int(0, 1), cellList[2]);
        Assert.AreEqual(new Vector2Int(0, 1), offset);
    }
}

delegate void ResetBlock (Vector2Int position, BlockType type, Vector2Int offset);
delegate void MoveLenght (int lenght);

class Reciever: BoardEventReceiver {

    public ResetBlock resetBlockD;
    public MoveLenght moveLenghtD;

    public void resetBlock (Vector2Int position, BlockType type, Vector2Int offset) {
        resetBlockD(position, type, offset);
    }

    public int moveLenght {
        set {
            moveLenghtD(value);
        }
    }
}
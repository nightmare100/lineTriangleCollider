# SpringBone - 三角形 - 胶囊体 碰撞算法修改 验证
```
绿色线段始终为 推力方向
```

### 3点一面 方向无误 支持任何角度
![avatar](https://github.com/nightmare100/lineTriangleCollider/blob/feature/klab/imgs/v1.gif)

### 正常碰撞
```
无论何种角度 裙子始终不会出现错误的方向 目前这个算法稳
```
![avatar](https://github.com/nightmare100/lineTriangleCollider/blob/feature/klab/imgs/v2.gif)

### 不正常碰撞-交叉碰撞1
```
用于 第一针帧膜的情况 以及巨大力量造成的不正常穿膜 （算法改完 估计也很难出现这种情况了吧，不过对于第一帧可能穿膜的动作 这个处理是必须的）
```
![avatar](https://github.com/nightmare100/lineTriangleCollider/blob/feature/klab/imgs/v3.gif)

### 不正常碰撞-交叉碰撞2
```
用于 之前在巨大力 或者大位移量 造成的 sibling穿过1 - 2个碰撞体 直接被挂在中间的 问题， 这样处理 完全不可能出现这种情况
```
![avatar](https://github.com/nightmare100/lineTriangleCollider/blob/feature/klab/imgs/v4.gif)

### SpringBone sibling 的 添加顺序
![avatar](https://github.com/nightmare100/lineTriangleCollider/blob/feature/klab/imgs/springbone-spt.png)
```
衣物腿部碰撞体设置顺序
rightupleg -> LeftLeg -> rightleg ->leftupleg
```


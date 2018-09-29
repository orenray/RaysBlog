﻿<script>
    //时间选择
    function pickDate(searchElem) {
        layui.use('laydate', function () {
            var laydate = layui.laydate;
            var $ = layui.jquery;
            laydate.render({
                elem: searchElem,
                theme: 'molv'
            });
        });
    };
    //分页
    //var pagesize = 3;
    layui.use(['laypage', 'layer'],
    function () {
        var laypage = layui.laypage,
        layer = layui.layer,
        $ = layui.jquery;

    //var count=$("#totalCount").val();
    //var pagecount = (count % pagesize) == 0 ? (count % pagesize) : (count % pagesize) + 1
        laypage.render({
        elem: 'pager',
    count: $("#pcount").val(),
    limit: $("#psize").val(),
    first: '首页',
    last: '尾页',
            prev: '<em>←</em>',
            next: '<em>→</em>',
            jump: function (obj, first) {
                var pageIndex = obj.curr;
                if (!first) {
                    var pageIndex = obj.curr;
    var size = obj.limit;
    layer.msg("点击了第" + pageIndex + "页")
    loadPageData(pageIndex, @ViewBag.TypeId);
}
//loadPageData(pageIndex);
}
});
});
    function loadPageData(pageIndex,itemId) {
        layui.use('layer', function () {
            var layer = layui.layer;
            var $ = layui.jquery;
            //var url = "/manage/item/id=" + itemId + "&pageIndex=" + pageIndex + "&ascending=true";
            var url = "/ray/item";
            var postData = { id: itemId, pageIndex: pageIndex, ascending: true };
            $.post(url, postData, function (htmlStr) {
                var html = htmlStr;
                //$("#ttab").empty();
                $("#ttab").html(html);
            }, "html");
        });
    };
    //查询
    function search() {
        layui.use(['laypage', 'layer'],
            function () {
                var laypage = layui.laypage,
                    layer = layui.layer,
                    $ = layui.jquery;
                var key = $("#search_key").val();
                var start = $("#search_start").val();
                var end = $("#search_end").val();
                var cabh = $("#search_cabh").val();
                var postData = { key: key, start: start, end: end, cabh: cabh };
                var url = "/Admin/Article/GetTotalCount?t=" + new Date().valueOf();
                $.post(url, postData, function (count) {
                    var pagecount = (count % pagesize) == 0 ? (count % pagesize) : (count % pagesize) + 1
                    laypage.render({
                        elem: 'pager',
                        count: count,
                        theme: '#1E9FFF',
                        limit: pagesize,
                        skip: pagecount,
                        layout: ['count', 'prev', 'page', 'next', 'limit', 'refresh', 'skip'],
                        jump: function (obj, first) {
                            var pageIndex = obj.curr;
                            pagesize = obj.limit;
                            if (!first) {
                                //var pageIndex = obj.curr;
                                //layer.msg("点击了第" + pageIndex + "页")
                            }
                            loadPageData(pageIndex, postData);
                        }
                    });
                });
            });
    };
    //删除
    function del(m) {
        var id = m.id;
    var tid = m.tid;
    layui.use('layer', function () {
        var layer = layui.layer;
    var $ = layui.jquery;
        layer.confirm("是否确认删除？", {
        btn: ['yes','no']@*yes or no 询问按钮*@
            }, function () {
            var url = "/ray/item/del"; @*post请求url*@
                $.post(url, {id: id,typeId:tid}, function (data) {
                @* function (data)请求成功后的回调函数  data后台回调的数据 *@
                    if (data.code == 2 || data.code == 1) {
                        if (!data.msg || data.msg == "") {
        layer.msg(retData.msg, { icon: 5 });
    }
}
                    if (data.code==0) {
        layer.msg(data.msg, {
            icon: 1,
            time: 500
        }, function () {
            self.location.reload(true);
        });
    }});
                }, function () {});
})
};
//弹出添加弹层
    function add(typeId) {
        layui.use('layer', function () {
            var $ = layui.jquery;
            layer = layui.layer;
            layer.open({
                type: 2,
                offset: 'auto',
                id: 'layerCheck' + 'auto',
                //content: '<div style="padding:20px 100px;">' + '<form class="layui-form"action=""><div class="layui-card"><div class="layui-card-header">添加</div><div class="layui-card-body"><form class="layui-form"action=""><div class="layui-row"><div class="layui-form-item"><label class="layui-form-label">分类标题</label><div class="layui-input-block"><input type="text"name="title"required lay-verify="required"placeholder="请输入标题"autocomplete="off"class="layui-input"></div></div><div class="layui-form-item"><label class="layui-form-label">密码框</label><div class="layui-input-inline"><input type="password"name="password"required lay-verify="required"placeholder="请输入密码"autocomplete="off"class="layui-input"></div><div class="layui-form-mid layui-word-aux">辅助文字</div></div><div class="layui-form-item layui-form-text"><label class="layui-form-label">文本域</label><div class="layui-input-block"><textarea name="desc"placeholder="请输入内容"class="layui-textarea"></textarea></div></div><div class="layui-form-item"><div class="layui-input-block"><button class="layui-btn"lay-submit lay-filter="formDemo">立即提交</button><button type="reset"class="layui-btn layui-btn-primary">重置</button></div></div></div></form>' + '</div>',
                content: "/ray/item/update?typeId=" + typeId,
                btnAlign: 'c',
                shade: [0.8, '#393D49'],
                anim: 0,
                maxmin: 1,
                closeBtn: 1,
                cancel: function (index, layero) {
                    if (confirm("确定关闭吗")) {
                        layer.close(index);
                    }
                    return false;
                },
                area: ['60%', '80%'],
                yes: function () {

                }
            });
            //$('layerCheck'.layui - btn).on('click', function () {
            //    var othis = $(this), method = othis.data('method');
            //    active(method) ? active[method].call(this, othis) : '';
            //});
        });
    }
</script>